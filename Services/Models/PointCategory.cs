using System.Collections.Generic;

namespace Shinra.Services.Models
{
    public class PointCategory
    {
        public string Name { get; set; }
        public double TotalPoints { get; set; }
        public Dictionary<string, double>SubCategories { get; set; } = new Dictionary<string, double>();
        public PointCategory(string name)
        {
            Name = name;
        }
    }
}
