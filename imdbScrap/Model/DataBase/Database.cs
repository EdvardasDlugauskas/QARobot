using System;
using System.Data;
using System.Data.SqlServerCe;
using Console = Colorful.Console;

namespace QARobot
{
    class Database
    {
        private static readonly SqlCeConnection Connection = new SqlCeConnection(SqlQueries.DbConnection);
        public static SqlCeConnection Connect
        {
            get
            {
                if (Connection.State == ConnectionState.Closed)
                    Connection.Open();

                return Connection;
            }
        }


        public static void CloseConnections()
        {
            if (Connect.State == ConnectionState.Open)
                Connect.Close();
        }

        public static void AddActorToDb(string name, string surname, string born)
        {
            try
            {
                var cmd = new SqlCeCommand(SqlQueries.AddActorToDb, Connect);
                cmd.Parameters.Add("@name", SqlDbType.NVarChar).Value = name;
                cmd.Parameters.Add("@surname", SqlDbType.NVarChar).Value = surname;
                cmd.Parameters.Add("@born", SqlDbType.NVarChar).Value = born;
                cmd.ExecuteNonQuery();
            }
            catch (Exception)
            {
                CloseConnections();
                Console.WriteLine("Count not insert. " + name + ", " + surname + ", " + born, ProfileManager.ErrorColor);
            }
        }

        public static void AddMovieToDb(string name, decimal rating, string genre, string year)
        {
            try
            {
                var cmd = new SqlCeCommand(SqlQueries.AddMovieToDb, Connect);
                cmd.Parameters.Add("@name", SqlDbType.NVarChar).Value = name;
                cmd.Parameters.Add("@rating", SqlDbType.Decimal, 2).Value = rating;
                cmd.Parameters.Add("@genre", SqlDbType.NVarChar).Value = genre;
                cmd.Parameters.Add("@year", SqlDbType.NVarChar).Value = year;
                cmd.ExecuteNonQuery();
            }
            catch (Exception)
            {
                CloseConnections();
                Console.WriteLine("Count not insert. " + name + ", " + rating + "," + genre + "," + year, ProfileManager.ErrorColor);
            }
        }


        public static int SelectActorId(string name, string surname)
        {
            var value = 0;
            try
            {
                using (
                    var command =
                        new SqlCeCommand(SqlQueries.SelectActorId(name, surname), Connect))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            for (var i = 0; i < reader.FieldCount; i++)
                            {
                                value = Convert.ToInt32(reader.GetValue(i));
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                CloseConnections();
                Console.WriteLine("Could not select " + name + " " + surname, ProfileManager.ErrorColor);
            }
            return value;
        }


        public static int SelectFilmId(string name)
        {
            var value = 0;
            try
            {
                using (var command = new SqlCeCommand(SqlQueries.SelectMovieId, Connect))
                {
                    command.Parameters.Add(new SqlCeParameter("Name", name));
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        value = reader.GetInt32(0);
                    }
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception)
            {
                CloseConnections();
                Console.WriteLine("Could not select " + name, ProfileManager.ErrorColor);
            }
            return value;
        }

        public static void SelectActorFilmId(int actorId, int filmId)
        {
            try
            {
                var cmd = new SqlCeCommand(SqlQueries.SelectActorMovieId, Connect);
                cmd.Parameters.Add("@actorId", SqlDbType.NVarChar).Value = actorId;
                cmd.Parameters.Add("@filmId", SqlDbType.NVarChar).Value = filmId;
                cmd.ExecuteNonQuery();
            }
            catch (Exception)
            {
                CloseConnections();
                Console.WriteLine("Count not insert " + actorId + ", " + filmId, ProfileManager.ErrorColor);
            }
        }

        public static void GetActorWithMostFilms()
        {
            try
            {
                using (var command = new SqlCeCommand(SqlQueries.GetActorWithMostFilms, Connect))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            for (var i = 0; i < reader.FieldCount; i++)
                            {
                                Console.Write(reader.GetValue(i) + " | ");
                            }
                            Console.WriteLine();
                            break;
                        }
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception)
            {
                CloseConnections();
                Console.WriteLine("Could not select actor with most films", ProfileManager.ErrorColor);
            }
        }

        public static void GetFilmWithBiggestRating()
        {
            try
            {
                using (var command = new SqlCeCommand(SqlQueries.GetFilmWithBiggestRating, Connect))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            for (var i = 0; i < reader.FieldCount; i++)
                            {
                                Console.Write(reader.GetValue(i) + " | ");
                            }
                            Console.WriteLine();
                            break;
                        }
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception)
            {
                CloseConnections();
                Console.WriteLine("Could not select actor with most films", ProfileManager.ErrorColor);
            }
        }

        public static void ActorsAndMovies(string query)
        {
            try
            {
                using (var command = new SqlCeCommand(
                    query, Connect))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            for (var i = 0; i < reader.FieldCount; i++)
                            {
                                Console.Write(reader.GetValue(i) + " | ");
                            }
                            Console.WriteLine();
                        }
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception)
            {
                CloseConnections();
                Console.WriteLine("Could not select actors", ProfileManager.ErrorColor);
            }
        }

        public static void FillDatabaseInfo(ActorScraper scraper)
        {
            Console.WriteLine("\r\nTransfering to database...", ProfileManager.InfoColor);

            foreach (var actor in scraper.UniqueActors)
            {
                AddActorToDb(actor.Name, actor.Surname, actor.Born);
            }

            foreach (var movie in scraper.UniqueFilms)
            {
                AddMovieToDb(movie.Name, movie.Rating, movie.Genre, movie.Year);
            }

            Console.WriteLine("Starting actors and films query...", ProfileManager.InfoColor);
            foreach (var actor in scraper.UniqueActors)
            {
                var actorId = SelectActorId(actor.Name, actor.Surname);
                foreach (var film in actor.Films)
                {
                    var movieId = SelectFilmId(film.Name);
                    SelectActorFilmId(actorId, movieId);
                }
            }
        }
    }
}
