using BeverageShop.API.Models;

namespace BeverageShop.API.Data
{
    public class BeverageShopData
    {
        private static List<Beverage> _beverages = new List<Beverage>
        {
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
                IsAvailable = true
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
                IsAvailable = true
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
                IsAvailable = true
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
                IsAvailable = true
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
                IsAvailable = true
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
                IsAvailable = true
            }
        };

        private static List<Category> _categories = new List<Category>
        {
            new Category { Id = 1, Name = "Trà", Description = "Các loại trà" },
            new Category { Id = 2, Name = "Cà phê", Description = "Các loại cà phê" },
            new Category { Id = 3, Name = "Nước ép", Description = "Nước ép trái cây tươi" },
            new Category { Id = 4, Name = "Sinh tố", Description = "Sinh tố hoa quả" },
            new Category { Id = 5, Name = "Soda", Description = "Soda và nước có ga" }
        };

        private static List<Order> _orders = new List<Order>();
        private static List<User> _users = new List<User>
        {
            new User
            {
                Id = 1,
                Username = "admin",
                Email = "admin@beverageshop.com",
                PasswordHash = Convert.ToBase64String(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes("admin123"))),
                FullName = "Administrator",
                Phone = "0123456789",
                Address = "Hà Nội",
                Role = UserRole.Admin
            }
        };
        private static List<Review> _reviews = new List<Review>();
        private static int _nextBeverageId = 7;
        private static int _nextOrderId = 1;
        private static int _nextUserId = 2;
        private static int _nextReviewId = 1;

        public static List<Beverage> GetBeverages() => _beverages;
        public static Beverage? GetBeverageById(int id) => _beverages.FirstOrDefault(b => b.Id == id);
        
        public static Beverage AddBeverage(Beverage beverage)
        {
            beverage.Id = _nextBeverageId++;
            beverage.CreatedDate = DateTime.Now;
            _beverages.Add(beverage);
            return beverage;
        }

        public static bool UpdateBeverage(Beverage beverage)
        {
            var existing = GetBeverageById(beverage.Id);
            if (existing == null) return false;

            existing.Name = beverage.Name;
            existing.Type = beverage.Type;
            existing.Category = beverage.Category;
            existing.Price = beverage.Price;
            existing.Size = beverage.Size;
            existing.Description = beverage.Description;
            existing.ImageUrl = beverage.ImageUrl;
            existing.Stock = beverage.Stock;
            existing.IsAvailable = beverage.IsAvailable;
            return true;
        }

        public static bool DeleteBeverage(int id)
        {
            var beverage = GetBeverageById(id);
            if (beverage == null) return false;
            return _beverages.Remove(beverage);
        }

        public static List<Category> GetCategories() => _categories;
        
        public static List<Order> GetOrders() => _orders;
        public static Order? GetOrderById(int id) => _orders.FirstOrDefault(o => o.Id == id);
        
        public static Order AddOrder(Order order)
        {
            order.Id = _nextOrderId++;
            order.OrderDate = DateTime.Now;
            _orders.Add(order);
            
            // Update stock
            foreach (var item in order.OrderItems)
            {
                var beverage = GetBeverageById(item.BeverageId);
                if (beverage != null)
                {
                    beverage.Stock -= item.Quantity;
                    if (beverage.Stock <= 0)
                    {
                        beverage.IsAvailable = false;
                    }
                }
            }
            
            return order;
        }

        public static bool UpdateOrderStatus(int orderId, OrderStatus status)
        {
            var order = GetOrderById(orderId);
            if (order == null) return false;
            order.Status = status;
            return true;
        }

        public static List<Order> GetOrdersByUserId(int userId) => 
            _orders.Where(o => o.UserId == userId).OrderByDescending(o => o.OrderDate).ToList();

        // User methods
        public static List<User> GetUsers() => _users;
        public static User? GetUserById(int id) => _users.FirstOrDefault(u => u.Id == id);
        public static User? GetUserByUsername(string username) => 
            _users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        
        public static User AddUser(User user)
        {
            user.Id = _nextUserId++;
            user.CreatedDate = DateTime.Now;
            _users.Add(user);
            return user;
        }

        public static bool UpdateUser(User user)
        {
            var existing = GetUserById(user.Id);
            if (existing == null) return false;

            existing.Email = user.Email;
            existing.FullName = user.FullName;
            existing.Phone = user.Phone;
            existing.Address = user.Address;
            return true;
        }

        // Review methods
        public static List<Review> GetReviews() => _reviews;
        public static List<Review> GetReviewsByBeverageId(int beverageId) => 
            _reviews.Where(r => r.BeverageId == beverageId).OrderByDescending(r => r.CreatedDate).ToList();
        
        public static Review AddReview(Review review)
        {
            review.Id = _nextReviewId++;
            review.CreatedDate = DateTime.Now;
            _reviews.Add(review);
            return review;
        }

        public static bool DeleteReview(int id)
        {
            var review = _reviews.FirstOrDefault(r => r.Id == id);
            if (review == null) return false;
            return _reviews.Remove(review);
        }

        // Statistics
        public static object GetDashboardStats()
        {
            var totalRevenue = _orders.Where(o => o.Status != OrderStatus.Cancelled).Sum(o => o.TotalAmount);
            var totalOrders = _orders.Count;
            var totalCustomers = _users.Count(u => u.Role == UserRole.Customer);
            var totalBeverages = _beverages.Count;
            var lowStockBeverages = _beverages.Where(b => b.Stock <= 10 && b.Stock > 0).ToList();
            var outOfStockBeverages = _beverages.Where(b => b.Stock == 0).ToList();

            var recentOrders = _orders.OrderByDescending(o => o.OrderDate).Take(10).ToList();
            var topSellingBeverages = _orders
                .Where(o => o.Status != OrderStatus.Cancelled)
                .SelectMany(o => o.OrderItems)
                .GroupBy(i => i.BeverageId)
                .Select(g => new
                {
                    BeverageId = g.Key,
                    Beverage = GetBeverageById(g.Key),
                    TotalSold = g.Sum(i => i.Quantity),
                    Revenue = g.Sum(i => i.Quantity * i.Price)
                })
                .OrderByDescending(x => x.TotalSold)
                .Take(5)
                .ToList();

            return new
            {
                totalRevenue,
                totalOrders,
                totalCustomers,
                totalBeverages,
                lowStockCount = lowStockBeverages.Count,
                outOfStockCount = outOfStockBeverages.Count,
                recentOrders,
                topSellingBeverages,
                lowStockBeverages,
                outOfStockBeverages
            };
        }
    }
}
