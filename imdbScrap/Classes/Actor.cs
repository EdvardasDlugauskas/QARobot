using System.Collections.Generic;
using System.Linq;

namespace QARobot
{
    public class Actor
    {
        public string Name;
        public string Surname;
        public HashSet<Film> Films;
        public string Born;

        public Actor(string name, string surname, string birthday)
        {
            Name = name;
            Surname = surname;
            Born = birthday;
            Films = new HashSet<Film>();
        }

        public Actor(string fullname)
        {
            var names = fullname.Split(' ');
            Name = names[0] ?? "";
            Surname = string.Join(" ", names.Skip(1)) ?? "";
            Born = "Someday...";
            Films = new HashSet<Film>();
        }

        public string Fullname => Name + " " + Surname;

        public override string ToString()
        {
            return $"{Name} {Surname} ({Born}) - {Films.Count} films";
        }

        public override bool Equals(object obj)
        {
            return ToString() == obj.ToString();
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
    }
}