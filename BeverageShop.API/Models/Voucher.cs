namespace BeverageShop.API.Models
{
    public enum DiscountType
    {
        Percentage,
        FixedAmount
    }

    public class Voucher
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DiscountType Type { get; set; }
        public decimal Value { get; set; }
        public decimal MaxDiscountAmount { get; set; } = 0;
        public decimal MinimumOrderAmount { get; set; }
        public int MaxUsageCount { get; set; } // 0 = unlimited
        public int UsedCount { get; set; } = 0;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; } = true;
        public string? ApplicableBeverageIds { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
