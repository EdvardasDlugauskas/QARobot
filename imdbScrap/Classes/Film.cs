using System.Dynamic;

namespace QARobot
{
    public class Film
    {
        public string Name { get; set; }
        public decimal Rating { get; set; }
        public string Genre { get; set; }
        public string Year { get; set; }

        public Film(string name, decimal rating, string year, string genre)
        {
            Name = name;
            Rating = rating;
            Year = year;
            Genre = genre;
        }

        public override string ToString()
        {
            return $"{Name} ({Year}) - {Rating}: {Genre}";
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