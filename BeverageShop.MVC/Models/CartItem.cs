namespace BeverageShop.MVC.Models
{
    public class CartItem
    {
        public Beverage Beverage { get; set; } = new Beverage();
        public int Quantity { get; set; }
        public decimal TotalPrice => Beverage.Price * Quantity;
    }
}
