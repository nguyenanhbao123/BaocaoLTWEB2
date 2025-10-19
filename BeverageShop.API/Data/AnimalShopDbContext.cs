using Microsoft.EntityFrameworkCore;
using BeverageShop.API.Models;

namespace BeverageShop.API.Data
{
    public class BeverageShopDbContext : DbContext
    {
        public BeverageShopDbContext(DbContextOptions<BeverageShopDbContext> options) 
            : base(options)
        {
        }

        public DbSet<Beverage> Beverages { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Wishlist> Wishlists { get; set; }
        public DbSet<Voucher> Vouchers { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<BlogPost> BlogPosts { get; set; }
        public DbSet<UserPoints> UserPoints { get; set; }
        public DbSet<PointTransaction> PointTransactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Order relationships
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany()
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.SetNull);  // Nếu xóa User, UserId sẽ = null

            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderItems)
                .WithOne()
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Review>()
                .HasOne<Beverage>()
                .WithMany()
                .HasForeignKey(r => r.BeverageId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Review>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Wishlist relationships
            modelBuilder.Entity<Wishlist>()
                .HasOne(w => w.User)
                .WithMany()
                .HasForeignKey(w => w.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Wishlist>()
                .HasOne(w => w.Beverage)
                .WithMany()
                .HasForeignKey(w => w.BeverageId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure UserPoints relationships
            modelBuilder.Entity<UserPoints>()
                .HasOne(up => up.User)
                .WithMany()
                .HasForeignKey(up => up.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Seed data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Categories
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Trà", Description = "Các loại trà" },
                new Category { Id = 2, Name = "Cà phê", Description = "Các loại cà phê" },
                new Category { Id = 3, Name = "Nước ép", Description = "Nước ép trái cây tươi" },
                new Category { Id = 4, Name = "Sinh tố", Description = "Sinh tố hoa quả" },
                new Category { Id = 5, Name = "Soda", Description = "Soda và nước có ga" }
            );

            // Seed Admin User
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "admin",
                    Email = "admin@beverageshop.com",
                    PasswordHash = Convert.ToBase64String(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes("admin123"))),
                    FullName = "Administrator",
                    Phone = "0123456789",
                    Address = "Hà Nội",
                    Role = UserRole.Admin,
                    CreatedDate = new DateTime(2025, 1, 1),
                    IsActive = true
                }
            );

            // Seed Beverages
            var seedDate = new DateTime(2025, 1, 1);
            modelBuilder.Entity<Beverage>().HasData(
                new Beverage
                {
                    Id = 1,
                    Name = "Trà Sữa Trân Châu",
                    Type = "Trà sữa",
                    Category = "Trà",
                    Price = 45000,
                    Size = "M",
                    Description = "Trà sữa trân châu đường đen thơm ngon, ngọt dịu",
                    ImageUrl = "/images/tra-sua-tran-chau.jpg",
                    Stock = 50,
                    IsAvailable = true,
                    CreatedDate = seedDate
                },
                new Beverage
                {
                    Id = 2,
                    Name = "Cà Phê Đen Đá",
                    Type = "Cà phê đen",
                    Category = "Cà phê",
                    Price = 25000,
                    Size = "M",
                    Description = "Cà phê đen nguyên chất, đậm đà hương vị Việt Nam",
                    ImageUrl = "/images/ca-phe-den.jpg",
                    Stock = 100,
                    IsAvailable = true,
                    CreatedDate = seedDate
                },
                new Beverage
                {
                    Id = 3,
                    Name = "Nước Ép Cam",
                    Type = "Nước ép",
                    Category = "Nước ép",
                    Price = 35000,
                    Size = "L",
                    Description = "Nước ép cam tươi 100%, giàu vitamin C",
                    ImageUrl = "/images/nuoc-ep-cam.jpg",
                    Stock = 30,
                    IsAvailable = true,
                    CreatedDate = seedDate
                },
                new Beverage
                {
                    Id = 4,
                    Name = "Sinh Tố Bơ",
                    Type = "Sinh tố",
                    Category = "Sinh tố",
                    Price = 40000,
                    Size = "L",
                    Description = "Sinh tố bơ béo ngậy, thơm ngon, bổ dưỡng",
                    ImageUrl = "/images/sinh-to-bo.jpg",
                    Stock = 25,
                    IsAvailable = true,
                    CreatedDate = seedDate
                },
                new Beverage
                {
                    Id = 5,
                    Name = "Trà Đào Cam Sả",
                    Type = "Trà trái cây",
                    Category = "Trà",
                    Price = 50000,
                    Size = "L",
                    Description = "Trà đào cam sả thanh mát, hương vị độc đáo",
                    ImageUrl = "/images/tra-dao-cam-sa.jpg",
                    Stock = 40,
                    IsAvailable = true,
                    CreatedDate = seedDate
                },
                new Beverage
                {
                    Id = 6,
                    Name = "Soda Chanh Dây",
                    Type = "Soda",
                    Category = "Soda",
                    Price = 30000,
                    Size = "M",
                    Description = "Soda chanh dây tươi mát, giải khát tuyệt vời",
                    ImageUrl = "/images/soda-chanh-day.jpg",
                    Stock = 60,
                    IsAvailable = true,
                    CreatedDate = seedDate
                }
            );

            // Seed Vouchers
            modelBuilder.Entity<Voucher>().HasData(
                new Voucher
                {
                    Id = 1,
                    Code = "WELCOME10",
                    Type = DiscountType.Percentage,
                    Value = 10,
                    MinimumOrderAmount = 0,
                    MaxUsageCount = 100,
                    UsedCount = 0,
                    StartDate = new DateTime(2025, 10, 1),
                    EndDate = new DateTime(2025, 12, 31),
                    IsActive = true,
                    Description = "Giảm 10% cho khách hàng mới - Không giới hạn đơn hàng"
                },
                new Voucher
                {
                    Id = 2,
                    Code = "FREESHIP",
                    Type = DiscountType.FixedAmount,
                    Value = 15000,
                    MinimumOrderAmount = 100000,
                    MaxUsageCount = 0, // Unlimited
                    UsedCount = 0,
                    StartDate = new DateTime(2025, 10, 1),
                    EndDate = new DateTime(2025, 12, 31),
                    IsActive = true,
                    Description = "Miễn phí ship cho đơn từ 100k"
                },
                new Voucher
                {
                    Id = 3,
                    Code = "VIP50K",
                    Type = DiscountType.FixedAmount,
                    Value = 50000,
                    MinimumOrderAmount = 200000,
                    MaxUsageCount = 50,
                    UsedCount = 0,
                    StartDate = new DateTime(2025, 10, 1),
                    EndDate = new DateTime(2025, 11, 30),
                    IsActive = true,
                    Description = "Giảm 50k cho đơn hàng từ 200k"
                },
                new Voucher
                {
                    Id = 4,
                    Code = "FLASH20",
                    Type = DiscountType.Percentage,
                    Value = 20,
                    MinimumOrderAmount = 150000,
                    MaxUsageCount = 20,
                    UsedCount = 0,
                    StartDate = new DateTime(2025, 10, 1),
                    EndDate = new DateTime(2025, 10, 7),
                    IsActive = true,
                    Description = "Flash Sale - Giảm 20% (Đơn từ 150k, giới hạn 20 lượt)"
                },
                new Voucher
                {
                    Id = 5,
                    Code = "DRINK15",
                    Type = DiscountType.Percentage,
                    Value = 15,
                    MinimumOrderAmount = 80000,
                    MaxUsageCount = 200,
                    UsedCount = 0,
                    StartDate = new DateTime(2025, 10, 1),
                    EndDate = new DateTime(2026, 12, 31),
                    IsActive = true,
                    Description = "Giảm 15% cho đồ uống yêu thích (Đơn từ 80k)"
                }
            );
        }
    }
}
