using System.Collections.Generic;

namespace Shinra.Services.Models
{
    public class PointCategory
    {
        public string CategoryName { get; set; }
        public double TotalPoints { get; set; }
        public List<PointSubCategory> SubCategories { get; set; } = new List<PointSubCategory>();
        public PointCategory(string categoryName)
        {
            CategoryName = categoryName;
        }
    }
}
