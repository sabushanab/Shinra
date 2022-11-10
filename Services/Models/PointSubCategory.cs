namespace Shinra.Services.Models
{
    public class PointSubCategory
    {
        public string CategoryName { get; set; }
        public double Points { get; set; }
        public PointSubCategory(string categoryName) 
        {
            CategoryName = categoryName;        
        }
    }
}
