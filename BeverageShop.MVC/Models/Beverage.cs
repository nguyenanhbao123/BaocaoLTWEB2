namespace BeverageShop.MVC.Models
{
    public class Beverage
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // Loại chi tiết
        public string Category { get; set; } = string.Empty; // Danh mục: Trà, Cà phê, Nước ép, Sinh tố, Soda
        public string Brand { get; set; } = string.Empty; // Thương hiệu: Trung Nguyên, Highlands, Nescafe...
        public decimal Price { get; set; }
        public string Size { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public int Stock { get; set; }
        public bool IsAvailable { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
