namespace BeverageShop.API.Models
{
    public class UserPoints
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int TotalPoints { get; set; } = 0;
        public int Level { get; set; } = 1;
        public string Badges { get; set; } = string.Empty; // JSON array of badge names
        public DateTime LastCheckIn { get; set; } = DateTime.Now;
        public int CheckInStreak { get; set; } = 0;

        // Navigation
        public User? User { get; set; }
    }

    public class PointTransaction
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int Points { get; set; }
        public string Action { get; set; } = string.Empty; // "purchase", "review", "check-in"
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
