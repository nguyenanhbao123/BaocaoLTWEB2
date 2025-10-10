namespace BeverageShop.API.Models
{
    public class Beverage
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // Loại: Trà, Cà phê, Nước ép...
        public string Category { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Size { get; set; } = string.Empty; // S, M, L
        public string Description { get; set; } = string.Empty;
        
        // Main image (for backward compatibility)
        public string ImageUrl { get; set; } = string.Empty;
        
        // Multiple images support (JSON array of URLs)
        public string? Images { get; set; }
        
        public int Stock { get; set; }
        public bool IsAvailable { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
