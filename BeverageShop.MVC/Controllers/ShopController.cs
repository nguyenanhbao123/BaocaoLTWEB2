using Microsoft.AspNetCore.Mvc;
using BeverageShop.MVC.Models;
using BeverageShop.MVC.Services;

namespace BeverageShop.MVC.Controllers
{
    public partial class ShopController : Controller
    {
        private readonly BeverageApiService _apiService;
        private const string CartSessionKey = "ShoppingCart";
        private const int PageSize = 6; // Số sản phẩm mỗi trang

        public ShopController(BeverageApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index(string? category, string? brand, int page = 1)
        {
            var allBeverages = await _apiService.GetBeveragesAsync(category);
            var categories = await _apiService.GetCategoriesAsync();
            
            // Filter by brand if provided
            if (!string.IsNullOrEmpty(brand))
            {
                allBeverages = allBeverages.Where(b => b.Brand == brand).ToList();
            }
            
            // Calculate pagination
            var totalItems = allBeverages.Count;
            var totalPages = (int)Math.Ceiling(totalItems / (double)PageSize);
            
            // Ensure page is within valid range
            page = Math.Max(1, Math.Min(page, totalPages == 0 ? 1 : totalPages));
            
            // Get items for current page
            var pagedBeverages = allBeverages
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToList();
            
            // Pass pagination info to view
            ViewBag.Categories = categories;
            ViewBag.SelectedCategory = category;
            ViewBag.SelectedBrand = brand;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalItems = totalItems;
            ViewBag.PageSize = PageSize;
            
            return View(pagedBeverages);
        }

        public async Task<IActionResult> Details(int id)
        {
            var beverage = await _apiService.GetBeverageByIdAsync(id);
            if (beverage == null)
            {
                return NotFound();
            }

            // Get reviews and stats
            var reviews = await _apiService.GetReviewsAsync(id);
            var stats = await _apiService.GetReviewStatsAsync(id);

            ViewBag.Reviews = reviews;
            ViewBag.ReviewStats = stats;

            return View(beverage);
        }

        [HttpPost]
        public async Task<IActionResult> AddReview(int beverageId, int rating, string comment)
        {
            var userJson = HttpContext.Session.GetString("CurrentUser");
            if (string.IsNullOrEmpty(userJson))
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập để đánh giá";
                return RedirectToAction("Login", "Account", new { returnUrl = $"/Shop/Details/{beverageId}" });
            }

            var user = System.Text.Json.JsonSerializer.Deserialize<User>(userJson);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var review = new Review
            {
                BeverageId = beverageId,
                UserId = user.Id,
                UserName = user.FullName,
                Rating = rating,
                Comment = comment
            };

            var success = await _apiService.CreateReviewAsync(review);
            if (success)
            {
                TempData["SuccessMessage"] = "Đánh giá của bạn đã được gửi!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể gửi đánh giá. Vui lòng thử lại!";
            }

            return RedirectToAction("Details", new { id = beverageId });
        }

        public async Task<IActionResult> Search(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return RedirectToAction(nameof(Index));
            }

            var beverages = await _apiService.SearchBeveragesAsync(keyword);
            ViewBag.Keyword = keyword;
            return View("Index", beverages);
        }

        public IActionResult Wishlist()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int beverageId, int quantity = 1)
        {
            var beverage = await _apiService.GetBeverageByIdAsync(beverageId);
            if (beverage == null)
            {
                return NotFound();
            }

            var cart = GetCart();
            var existingItem = cart.FirstOrDefault(c => c.Beverage.Id == beverageId);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                cart.Add(new CartItem
                {
                    Beverage = beverage,
                    Quantity = quantity
                });
            }

            SaveCart(cart);
            return RedirectToAction(nameof(Cart));
        }

        [HttpPost]
        public async Task<IActionResult> AddToWishlist(int beverageId)
        {
            var userJson = HttpContext.Session.GetString("CurrentUser");
            if (string.IsNullOrEmpty(userJson))
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập để thêm vào yêu thích";
                return RedirectToAction("Login", "Account", new { returnUrl = $"/Shop/Details/{beverageId}" });
            }

            var user = System.Text.Json.JsonSerializer.Deserialize<User>(userJson);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var wishlist = new Wishlist
            {
                UserId = user.Id,
                BeverageId = beverageId
            };

            var success = await _apiService.AddToWishlistAsync(wishlist);
            if (success)
            {
                TempData["SuccessMessage"] = "Đã thêm vào danh sách yêu thích";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể thêm vào wishlist. Vui lòng thử lại.";
            }

            return RedirectToAction("Details", new { id = beverageId });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromWishlist(int wishlistId)
        {
            var userJson = HttpContext.Session.GetString("CurrentUser");
            if (string.IsNullOrEmpty(userJson))
            {
                return RedirectToAction("Login", "Account");
            }

            var success = await _apiService.RemoveFromWishlistAsync(wishlistId);
            if (success)
            {
                TempData["SuccessMessage"] = "Đã xóa khỏi wishlist";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể xóa từ wishlist. Vui lòng thử lại.";
            }

            return RedirectToAction("Wishlist");
        }

        public IActionResult Cart()
        {
            var cart = GetCart();
            return View(cart);
        }

        [HttpPost]
        public IActionResult UpdateCart(int beverageId, int quantity)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(c => c.Beverage.Id == beverageId);

            if (item != null)
            {
                if (quantity <= 0)
                {
                    cart.Remove(item);
                }
                else
                {
                    item.Quantity = quantity;
                }
            }

            SaveCart(cart);
            return RedirectToAction(nameof(Cart));
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int beverageId)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(c => c.Beverage.Id == beverageId);

            if (item != null)
            {
                cart.Remove(item);
            }

            SaveCart(cart);
            return RedirectToAction(nameof(Cart));
        }

        public IActionResult Checkout()
        {
            var cart = GetCart();
            if (!cart.Any())
            {
                return RedirectToAction(nameof(Cart));
            }

            return View(cart);
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder(string customerName, string customerEmail, 
            string customerPhone, string shippingAddress, string? voucherCode, decimal discountAmount = 0)
        {
            var cart = GetCart();
            if (!cart.Any())
            {
                return RedirectToAction(nameof(Cart));
            }

            var subtotal = cart.Sum(c => c.TotalPrice);
            var finalTotal = subtotal - discountAmount;

            var order = new Order
            {
                CustomerName = customerName,
                CustomerEmail = customerEmail,
                CustomerPhone = customerPhone,
                ShippingAddress = shippingAddress,
                TotalAmount = finalTotal,
                VoucherCode = voucherCode,
                DiscountAmount = discountAmount,
                OrderItems = cart.Select(c => new OrderItem
                {
                    BeverageId = c.Beverage.Id,
                    BeverageName = c.Beverage.Name,
                    Quantity = c.Quantity,
                    Price = c.Beverage.Price
                }).ToList(),
                OrderDate = DateTime.Now,
                Status = OrderStatus.Pending,
                IsPaid = true // mark paid since payment step completed
            };

            // If user is logged in, associate order with user so it appears in their Orders list
            var userJson = HttpContext.Session.GetString("CurrentUser");
            if (!string.IsNullOrEmpty(userJson))
            {
                var user = System.Text.Json.JsonSerializer.Deserialize<User>(userJson);
                if (user != null)
                {
                    order.UserId = user.Id;
                }
            }

            var result = await _apiService.CreateOrderAsync(order);
            
            if (result != null)
            {
                ClearCart();
                return View("OrderSuccess", result);
            }

            ViewBag.Error = "Không thể tạo đơn hàng. Vui lòng thử lại.";
            return View("Checkout", cart);
        }

        private List<CartItem> GetCart()
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>(CartSessionKey);
            return cart ?? new List<CartItem>();
        }

        private void SaveCart(List<CartItem> cart)
        {
            HttpContext.Session.SetObjectAsJson(CartSessionKey, cart);
        }

        private void ClearCart()
        {
            HttpContext.Session.Remove(CartSessionKey);
        }

        public IActionResult ManageImages()
        {
            return View();
        }
    }

    // Extension methods for Session
    public static class SessionExtensions
    {
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            session.SetString(key, System.Text.Json.JsonSerializer.Serialize(value));
        }

        public static T? GetObjectFromJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default : System.Text.Json.JsonSerializer.Deserialize<T>(value);
        }
    }
}
