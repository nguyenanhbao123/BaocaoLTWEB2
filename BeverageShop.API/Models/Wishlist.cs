namespace BeverageShop.API.Models
{
    public class Wishlist
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int BeverageId { get; set; }
        public DateTime AddedDate { get; set; } = DateTime.Now;

        // Navigation properties
        public User? User { get; set; }
        public Beverage? Beverage { get; set; }
    }
}
