namespace BeverageShop.API.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public string ShippingAddress { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public decimal TotalAmount { get; set; }
        public string? VoucherCode { get; set; }
        public decimal DiscountAmount { get; set; } = 0;
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public string PaymentMethod { get; set; } = "COD";
        public bool IsPaid { get; set; } = false;
        public DateTime? PaidDate { get; set; }
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }

    public enum OrderStatus
    {
        Pending,
        Confirmed,
        Processing,
        Shipping,
        Delivered,
        Cancelled
    }

    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int BeverageId { get; set; }
        public string BeverageName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
