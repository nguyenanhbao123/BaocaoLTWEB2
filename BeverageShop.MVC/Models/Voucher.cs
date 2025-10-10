namespace BeverageShop.MVC.Models
{
    public class Voucher
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public VoucherType Type { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal MaxDiscountAmount { get; set; }
        public decimal MinimumOrderAmount { get; set; }
        public int MaxUsageCount { get; set; }
        public int UsedCount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; } = true;
        public string? ApplicableBeverageIds { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }

    public enum VoucherType
    {
        Percentage,  // Giảm theo %
        FixedAmount  // Giảm số tiền cố định
    }
}
