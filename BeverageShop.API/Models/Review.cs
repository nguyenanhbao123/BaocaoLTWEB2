namespace BeverageShop.API.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int BeverageId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int Rating { get; set; } // 1-5 stars
        public string Comment { get; set; } = string.Empty;
        public string? Images { get; set; } // JSON array of image URLs
        public bool IsVerifiedPurchase { get; set; } = false;
        public int HelpfulCount { get; set; } = 0;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
