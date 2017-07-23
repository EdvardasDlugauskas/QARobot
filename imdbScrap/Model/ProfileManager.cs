using System.Drawing;
using OpenQA.Selenium.Firefox;
using Console = Colorful.Console;

namespace QARobot
{
    class ProfileManager
    {
        public static readonly Color InfoColor = Color.NavajoWhite;
        public static readonly Color ErrorColor = Color.IndianRed;
        public static readonly Color SuccessColor = Color.LimeGreen;
        public static readonly Color ResultColor = Color.CadetBlue;

        public static string ChromeOnLinuxProfile =
            "Mozilla/5.0 (X11; Linux i686) AppleWebKit/535.11 (KHTML, like Gecko) Ubuntu/11.10 Chromium/17.0.963.65 Chrome/17.0.963.65 Safari/535.11";

        public static string IeOniOsProfile =
            "Mozilla/5.0 (compatible; MSIE 10.0;CPU iPhone OS 3_2 like Mac OS X; en - us) AppleWebKit / 531.21.20(KHTML, like Gecko) Mobile / 7B298g";

        public static string SafariOnMacProfile =
            "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_9_3) AppleWebKit/537.75.14 (KHTML, like Gecko) Version/7.0.3 Safari/7046A194A";

        public static string DefaultProfile = "default";

        public static string EmptyProfile = "";

        public static FirefoxProfile ChangeProfileUserAgent(string userAgent)
        {
            var myProfile = new FirefoxProfile();
            myProfile.SetPreference("general.useragent.override", userAgent);
            return myProfile;
        }

        public static FirefoxProfile CookieProfiles(string cookieProfile)
        {
            var profile = new FirefoxProfile();
            if (cookieProfile != string.Empty)
            {
                var myProfile = new FirefoxProfileManager();
                profile = myProfile.GetProfile(cookieProfile);

            }
            return profile;
        }

        public static FirefoxProfile GetProfile()
        {
            Console.WriteLine(@"Welcome. Please select profile to use for scrapper:
                              1) Chrome on Linux;
                              2) Internet explorer on iOS;
                              3) Safari on Mac;
                              4) User default;
                              5) No profile;", InfoColor);

            var input = Console.ReadKey().KeyChar.ToString();
            int selectionKey;
            int.TryParse(input, out selectionKey);
            var done = false;
            var profile = new FirefoxProfile();
            while (!done)
            {
                switch (selectionKey)
                {
                    case 1:
                        Console.WriteLine("\r\nYou have selected Chrome on Linux profile.", InfoColor);
                        profile = ChangeProfileUserAgent(ChromeOnLinuxProfile);
                        done = true;
                        break;
                    case 2:
                        Console.WriteLine("\r\nYou have selected Internet explorer on iOS profile.", InfoColor);
                        profile = ChangeProfileUserAgent(IeOniOsProfile);
                        done = true;
                        break;
                    case 3:
                        Console.WriteLine("\r\nYou have selected Safari on Mac profile.", InfoColor);
                        profile = ChangeProfileUserAgent(SafariOnMacProfile);
                        done = true;
                        break;
                    case 4:
                        Console.WriteLine("\r\nYou have selected user default profile.", InfoColor);
                        profile = CookieProfiles(DefaultProfile);
                        done = true;
                        break;
                    case 5:
                        Console.WriteLine("\r\nYou have selected selenium profile.", InfoColor);
                        profile = CookieProfiles(EmptyProfile);
                        done = true;
                        break;
                    default:
                        Console.WriteLine("\r\nIncorrect value. Please enter 1,2,3,4 or 5.", ErrorColor);
                        input = Console.ReadKey().KeyChar.ToString();
                        int.TryParse(input, out selectionKey);
                        break;
                }
            }
            return profile;
        }
    }
}
