namespace BeverageShop.MVC.Models
{
    public class Wishlist
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int BeverageId { get; set; }
        public DateTime AddedDate { get; set; }
    }
}
