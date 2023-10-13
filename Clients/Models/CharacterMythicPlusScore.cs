namespace Shinra.Clients.Models
{
    public class CharacterMythicPlusScore
    {
        public CurrentMythicRating current_mythic_rating { get; set; }
    }

    public class CurrentMythicRating
    {
        public Color color { get; set; }
        public double rating { get; set; }
    }

    public class Color
    {
        public int r { get; set; }
        public int g { get; set; }
        public int b { get; set; }
        public double a { get; set; }
    }
}
