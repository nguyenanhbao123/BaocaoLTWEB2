namespace BeverageShop.API.Models
{
    public class BlogPost
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public string ThumbnailUrl { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Tags { get; set; } = string.Empty; // Comma-separated
        public int AuthorId { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public int ViewCount { get; set; } = 0;
        public bool IsPublished { get; set; } = false;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? PublishedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
