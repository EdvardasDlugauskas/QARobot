using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Console = Colorful.Console;
using System.Text;

namespace QARobot
{
    class SqlQueries
    {
        public static Dictionary<string, string> ChoosenActorsDictionary = new Dictionary<string, string>();
        public static List<Actor> ActorObjList = new List<Actor>();
        public static List<string> ActorsList;
        public static string DbConnection = @"
                    Data Source = database.sdf";

        public static string AddActorToDb = @"
                    INSERT INTO actor(Name, Surname, Born) 
                    VALUES(@name, @surname, @born)";

        public static string AddMovieToDb = @"
                    INSERT INTO Film(Name, Rating, Genre, Year)
                    VALUES(@name, @rating, @genre, @year)";

        public static string SelectMovieId = @"
                    SELECT [filmId]
                    FROM [Film] 
                    WHERE [name] = @name";

        public static string SelectActorMovieId = @"
                    INSERT INTO FilmaiToActor(actorId, filmId)
                    VALUES(@actorId, @filmId)";

        public static string GetActorWithMostFilms = @"
                    SELECT  a.Name,a.surname, COUNT(f.name) AS filmuSk
                    FROM FilmaiToActor AS fa 
                    INNER JOIN Film AS f ON fa.filmId = f.filmId
                    INNER JOIN actor AS a ON fa.ActorId = a.actorId 
                    GROUP BY a.Name, a.surname
                    ORDER BY filmuSk DESC";

        public static string GetFilmWithBiggestRating = @"
                    SELECT a.Name, a.surname, f.name, MAX(f.rating) AS filmuRating
                    FROM FilmaiToActor AS fa 
                    INNER JOIN Film AS f ON fa.filmId = f.filmId  
                    INNER JOIN actor AS a ON fa.ActorId=a.actorId 
                    GROUP BY a.Name, a.surname, f.name 
                    ORDER BY filmuRating DESC";

        public static string SelectActorId(string name, string surname)
        {
            return @"
                SELECT [actorID] 
                FROM [actor]
                WHERE [name]='" + name + "' " +
                "AND [surname]='" + surname + "'";
        }

        public static string UniversalString(List<Actor> actorList = null)
        {
            if (actorList == null) actorList = ActorObjList;
            var cmd = new SqlCommand();
            var sqlBuilder = new StringBuilder();
            sqlBuilder.Append("SELECT  f.name, f.year, f.rating  " +
                              "FROM FilmaiToActor AS fa " +
                              "INNER JOIN Film AS f ON fa.filmId = f.filmId " +
                              "INNER JOIN actor AS a ON fa.ActorId=a.actorId ");
            if (actorList.Count > 1)
            {
                var i = 1;
                foreach (var item in actorList)
                {
                    sqlBuilder.Append(i == 1 ? " WHERE " : " OR ");
                    var paramName = item.Name;
                    var paramSurname = item.Surname;
                    sqlBuilder.AppendFormat("(a.Name ='{0}' AND a.Surname = '{1}' )", paramName, paramSurname);
                    cmd.Parameters.AddWithValue(paramName, "%" + item + "%");
                    cmd.Parameters.AddWithValue(paramSurname, "%" + item + "%");
                    i++;
                }
            }

            return cmd.CommandText = sqlBuilder + "GROUP BY  f.name, f.year, f.rating " +
                                     "HAVING COUNT(*) >=" + actorList.Count;
        }

        public static string CoStarMethod()
        {
            Console.WriteLine("\r\nChoose Co-star actors :", ProfileManager.ResultColor);
            for (var i = 0; i < ActorObjList.Count; i++)
            {
                Console.WriteLine(i + ". "+ ActorObjList[i].Fullname, ProfileManager.InfoColor);
            }

            var readQuantity = "";
            var chosenActors = new HashSet<Actor>();
            int quantity;
            while (!IsStringIntRange(readQuantity, 2, ActorObjList.Count))
            {
                Console.WriteLine($"\r\nHow many actors you would like to Co-Star? (No less than 2, no more than {ActorObjList.Count})", ProfileManager.InfoColor);
                readQuantity = Console.ReadKey().KeyChar.ToString();
            }

            int.TryParse(readQuantity, out quantity);

            while (chosenActors.Count < quantity)
            {
                var actorIndex = "";

                while (!IsStringIntRange(actorIndex, 0, ActorObjList.Count - 1))
                {
                    Console.WriteLine($"\r\nPlease enter index of actor #{chosenActors.Count + 1}: ", ProfileManager.InfoColor);
                    actorIndex = Console.ReadLine();
                }

                chosenActors.Add(ActorObjList[int.Parse(actorIndex)]);
            }
            Console.WriteLine("\r\nYour chosen actors co-starred in all these films:", ProfileManager.InfoColor);
            return UniversalString(chosenActors.ToList());
        }

        /// <summary>
        /// Inclusive int.parse(string) check in boundary [lower, upper]
        /// </summary>
        /// <returns>True if in boundary, false if not (or if incorrect string supplied)</returns>
        public static bool IsStringIntRange(string intString, int lower, int upper)
        {
            try
            {
                var integer = int.Parse(intString);
                if (integer >= lower && integer <= upper) return true;
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return false;
            }
        }
    }
}
