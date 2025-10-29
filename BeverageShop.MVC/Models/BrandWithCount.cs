namespace BeverageShop.MVC.Models
{
    public class BrandWithCount
    {
        public string Name { get; set; } = string.Empty;
        public int Count { get; set; }
        public int TotalStock { get; set; }
    }
}
