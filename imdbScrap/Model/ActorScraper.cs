using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using Console = Colorful.Console;

namespace QARobot
{
    class ActorScraper
    {
        private readonly IWebDriver _driver;
        private readonly string _baseUrl = "http://www.imdb.com";

        readonly NumberFormatInfo _decimalFormat = new NumberFormatInfo();
        private WebDriverWait _wait;

        public static readonly string imdbApiTemplate = 
            "http://www.imdb.com/xml/find?json=1&nr=1&nm=on&q={0}";

        readonly string _imdbActorFilmsTemplate =
            "http://www.imdb.com/filmosearch?explore=title_type&role={0}&title_type=movie";

        public HashSet<Actor> UniqueActors = new HashSet<Actor>();
        public HashSet<Film> UniqueFilms = new HashSet<Film>();

        public ActorScraper(FirefoxProfile profile=null)
        {
            try
            {
                _driver = new FirefoxDriver(profile);
            }
            catch (Exception)
            {
                Console.WriteLine("Sorry, couldn't access default Firefox profile. Using Selenium profile (empty) instead.", ProfileManager.ErrorColor);
                _driver = new FirefoxDriver();
            }

            _driver.Manage().Window.Maximize();
            _decimalFormat.NumberDecimalSeparator = ".";
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));

        }

        public List<Actor> GetScrapedActors()
        {
            return UniqueActors.ToList();
        }

        public List<Film> GetFilmsInCommon()
        {
            var common = new HashSet<Film>(UniqueActors.First().Films);
            foreach (var actor in UniqueActors)
            {
                common.IntersectWith(actor.Films);
            }
            return common.ToList();
        }

        public List<Film> GetScrapedFilms()
        {
            return UniqueFilms.ToList();
        }

        /// <summary>
        /// Fills the ActorScraper object with info by using a dict of actorName, actorImbdNumber strings.
        /// </summary>
        /// <param name="actorDict"></param>
        public void ScrapeActors(Dictionary<string, string> actorDict)
        {
            //Console.WriteLine("\r\nStarting IMDB scraping...", ProfileManager.InfoColor);

            foreach (var actor in actorDict)
            {
                var actorFullname = actor.Key;
                var actorName = actorFullname.Split(' ')[0];
                var actorSurname = string.Join(" ", actorFullname.Split(' ').Skip(1));

                var actorNumber = actor.Value;

                _driver.Navigate().GoToUrl(_baseUrl + "/name/" + actorNumber);
                var actorBirthday = string.Empty;
                try
                {
                    actorBirthday =
                    _driver.FindElement(By.XPath(".//*[@id=\'name-born-info\']/time")).GetAttribute("datetime");
                }
                catch (NoSuchElementException) { }

                _driver.Navigate().GoToUrl(string.Format(_imdbActorFilmsTemplate, actorNumber));

                var currentActor = new Actor(actorName, actorSurname, actorBirthday);

                // Scrape films from page
                while (true)
                {
                    foreach (var filmElem in _driver.FindElements(By.XPath("//*[contains(@class,\'lister-item-content\')]")))
                    {
                        var filmName =
                            filmElem.FindElement(By.ClassName("lister-item-header")).FindElement(By.TagName("a")).Text;

                        decimal filmRating = 0;
                        try
                        {
                            var filmRatingStr =
                                filmElem.FindElement(By.ClassName("ratings-imdb-rating")).FindElement(By.TagName("strong")).Text.Replace(',', '.');
                            filmRating = decimal.Parse(filmRatingStr, _decimalFormat);
                        }
                        catch (NoSuchElementException) { }

                        var filmYear = string.Empty;
                        try
                        {
                            var text =
                                filmElem.FindElement(
                                    By.XPath(".//*[contains(@class, \'lister-item-year text-muted unbold\')]")).Text;
                            filmYear = Regex.Match(text, @"\((\d{4})\)").Groups[1].Value;
                        }
                        catch (NoSuchElementException) { }

                        var filmGenre = string.Empty;
                        try
                        {
                            filmGenre =
                                filmElem.FindElement(By.XPath(".//*[contains(@class, \'genre\')]")).Text.Trim();
                        }
                        catch (NoSuchElementException) { }

                        var currentFilm = new Film(filmName, filmRating, filmYear, filmGenre);

                        currentActor.Films.Add(currentFilm);
                        UniqueFilms.Add(currentFilm);
                    }

                    try
                    {
                        var nextBtn = _driver.FindElement(By.ClassName("next-page"));
                        nextBtn.Click();
                        _wait.Until(ExpectedConditions.StalenessOf(nextBtn));
                    }
                    catch (NoSuchElementException)
                    {
                        break;
                    }


                }

                UniqueActors.Add(currentActor);
            }
            _driver.Quit();
            //Console.WriteLine("Scraping complete.", ProfileManager.SuccessColor);
        }
    }
}

