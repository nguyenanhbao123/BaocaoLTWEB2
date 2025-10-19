using Microsoft.AspNetCore.Mvc;
using BeverageShop.MVC.Models;
using BeverageShop.MVC.Services;
using System.Text.Json;

namespace BeverageShop.MVC.Controllers
{
    public class AdminController : Controller
    {
        private readonly BeverageApiService _apiService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public AdminController(BeverageApiService apiService, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _apiService = apiService;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        // Middleware to check admin access
        private bool IsAdmin()
        {
            var userJson = HttpContext.Session.GetString("CurrentUser");
            if (string.IsNullOrEmpty(userJson))
                return false;

            var user = JsonSerializer.Deserialize<User>(userJson);
            return user?.Role == UserRole.Admin;
        }

        // Dashboard
        public async Task<IActionResult> Index()
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Account", new { returnUrl = "/Admin" });
            }

            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync("http://localhost:5000/api/admin/dashboard");
            
            if (response.IsSuccessStatusCode)
            {
                var dashboard = await response.Content.ReadFromJsonAsync<DashboardStats>();
                return View(dashboard);
            }

            return View(new DashboardStats());
        }

        // === QUẢN LÝ NGƯỜI DÙNG ===
        public async Task<IActionResult> Users()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync("http://localhost:5000/api/admin/users");
            
            if (response.IsSuccessStatusCode)
            {
                var users = await response.Content.ReadFromJsonAsync<List<User>>();
                return View("Users/Users", users);
            }

            return View("Users/Users", new List<User>());
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUserRole(int userId, string role)
        {
            if (!IsAdmin()) return Unauthorized();

            var client = _httpClientFactory.CreateClient();
            var response = await client.PutAsJsonAsync($"http://localhost:5000/api/admin/users/{userId}/role", new { role });

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Cập nhật quyền thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể cập nhật quyền!";
            }

            return RedirectToAction(nameof(Users));
        }

        [HttpPost]
        public async Task<IActionResult> ToggleUserStatus(int userId)
        {
            if (!IsAdmin()) return Unauthorized();

            var client = _httpClientFactory.CreateClient();
            var response = await client.PutAsync($"http://localhost:5000/api/admin/users/{userId}/toggle-status", null);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Cập nhật trạng thái thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể cập nhật trạng thái!";
            }

            return RedirectToAction(nameof(Users));
        }

        public IActionResult CreateUser()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            return View("Users/CreateUser");
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(User user, string password)
        {
            if (!IsAdmin()) return Unauthorized();

            var client = _httpClientFactory.CreateClient();
            var response = await client.PostAsJsonAsync("http://localhost:5000/api/auth/register", new
            {
                username = user.Username,
                email = user.Email,
                password = password,
                fullName = user.FullName,
                phone = user.Phone,
                address = user.Address
            });

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Thêm người dùng thành công!";
                return RedirectToAction(nameof(Users));
            }

            var error = await response.Content.ReadAsStringAsync();
            TempData["ErrorMessage"] = $"Không thể thêm người dùng! {error}";
            return View("Users/CreateUser", user);
        }

        public async Task<IActionResult> EditUser(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"http://localhost:5000/api/admin/users");
            
            if (response.IsSuccessStatusCode)
            {
                var users = await response.Content.ReadFromJsonAsync<List<User>>();
                var user = users?.FirstOrDefault(u => u.Id == id);
                if (user != null)
                {
                    return View("Users/EditUser", user);
                }
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(int id, User user)
        {
            if (!IsAdmin()) return Unauthorized();

            var client = _httpClientFactory.CreateClient();
            var response = await client.PutAsJsonAsync($"http://localhost:5000/api/admin/users/{id}", user);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Cập nhật người dùng thành công!";
                return RedirectToAction(nameof(Users));
            }

            var error = await response.Content.ReadAsStringAsync();
            TempData["ErrorMessage"] = $"Không thể cập nhật người dùng! {error}";
            return View("Users/EditUser", user);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(int id)
        {
            if (!IsAdmin()) return Unauthorized();

            var client = _httpClientFactory.CreateClient();
            var response = await client.DeleteAsync($"http://localhost:5000/api/admin/users/{id}");

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Xóa người dùng thành công!";
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                TempData["ErrorMessage"] = $"Không thể xóa người dùng! {error}";
            }

            return RedirectToAction(nameof(Users));
        }

        // === QUẢN LÝ SẢN PHẨM ===
        public async Task<IActionResult> Beverages()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var beverages = await _apiService.GetBeveragesAsync(null);
            return View("Products/Beverages", beverages);
        }

        public async Task<IActionResult> CreateBeverage()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var categories = await _apiService.GetCategoriesAsync();
            ViewBag.Categories = categories;
            return View("Products/CreateBeverage");
        }

        [HttpPost]
        public async Task<IActionResult> CreateBeverage(Beverage beverage)
        {
            if (!IsAdmin()) return Unauthorized();

            var client = _httpClientFactory.CreateClient();
            var response = await client.PostAsJsonAsync("http://localhost:5000/api/beverages", beverage);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Thêm sản phẩm thành công!";
                return RedirectToAction(nameof(Beverages));
            }

            TempData["ErrorMessage"] = "Không thể thêm sản phẩm!";
            var categories = await _apiService.GetCategoriesAsync();
            ViewBag.Categories = categories;
            return View("Products/CreateBeverage", beverage);
        }

        public async Task<IActionResult> EditBeverage(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var beverage = await _apiService.GetBeverageByIdAsync(id);
            if (beverage == null) return NotFound();

            var categories = await _apiService.GetCategoriesAsync();
            ViewBag.Categories = categories;
            return View("Products/EditBeverage", beverage);
        }

        [HttpPost]
        public async Task<IActionResult> EditBeverage(int id, Beverage beverage)
        {
            if (!IsAdmin()) return Unauthorized();

            var client = _httpClientFactory.CreateClient();
            var response = await client.PutAsJsonAsync($"http://localhost:5000/api/beverages/{id}", beverage);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Cập nhật sản phẩm thành công!";
                return RedirectToAction(nameof(Beverages));
            }

            TempData["ErrorMessage"] = "Không thể cập nhật sản phẩm!";
            return View("Products/EditBeverage", beverage);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteBeverage(int id)
        {
            if (!IsAdmin()) return Unauthorized();

            var client = _httpClientFactory.CreateClient();
            var response = await client.DeleteAsync($"http://localhost:5000/api/beverages/{id}");

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Xóa sản phẩm thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể xóa sản phẩm!";
            }

            return RedirectToAction(nameof(Beverages));
        }

        // === QUẢN LÝ ĐỠN HÀNG ===
        public async Task<IActionResult> Orders()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync("http://localhost:5000/api/orders");
            
            if (response.IsSuccessStatusCode)
            {
                var orders = await response.Content.ReadFromJsonAsync<List<Order>>();
                return View("Order/Orders", orders);
            }

            return View("Order/Orders", new List<Order>());
        }

        public async Task<IActionResult> OrderDetails(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"http://localhost:5000/api/orders/{id}");
            
            if (response.IsSuccessStatusCode)
            {
                var order = await response.Content.ReadFromJsonAsync<Order>();
                return View("Order/OrderDetails", order);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateOrderStatus(int id, string status)
        {
            if (!IsAdmin()) return Unauthorized();

            var client = _httpClientFactory.CreateClient();
            var response = await client.PutAsJsonAsync($"http://localhost:5000/api/admin/orders/{id}/status", new { status });

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Cập nhật trạng thái đơn hàng thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể cập nhật trạng thái!";
            }

            return RedirectToAction(nameof(OrderDetails), new { id });
        }

        // === QUẢN LÝ THƯƠNG HIỆU (BRANDS) ===
        public async Task<IActionResult> Brands()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{_configuration["ApiSettings:BaseUrl"]}/api/brands/with-count");

            if (response.IsSuccessStatusCode)
            {
                var brands = await response.Content.ReadFromJsonAsync<List<dynamic>>();
                return View("Brand/Brands", brands);
            }

            return View("Brand/Brands", new List<dynamic>());
        }

        public async Task<IActionResult> BeveragesByBrand(string brand)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var beverages = await _apiService.GetBeveragesAsync();
            var filtered = beverages.Where(b => b.Brand == brand).ToList();
            
            ViewBag.BrandName = brand;
            return View("Brand/BeveragesByBrand", filtered);
        }

        public IActionResult CreateBrand()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            return View("Brand/CreateBrand");
        }

        public IActionResult EditBrand(string brand)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            return View("Brand/EditBrand", brand);
        }

        // === QUẢN LÝ DANH MỤC ===
        public async Task<IActionResult> Categories()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var categories = await _apiService.GetCategoriesAsync();
            return View(categories);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory(string name, string? description)
        {
            if (!IsAdmin()) return Unauthorized();

            var client = _httpClientFactory.CreateClient();
            var response = await client.PostAsJsonAsync("http://localhost:5000/api/categories", new { name, description });

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Thêm danh mục thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể thêm danh mục!";
            }

            return RedirectToAction(nameof(Categories));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            if (!IsAdmin()) return Unauthorized();

            var client = _httpClientFactory.CreateClient();
            var response = await client.DeleteAsync($"http://localhost:5000/api/categories/{id}");

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Xóa danh mục thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể xóa danh mục!";
            }

            return RedirectToAction(nameof(Categories));
        }

        // === QUẢN LÝ MÃ GIẢM GIÁ ===
        public async Task<IActionResult> Vouchers()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync("http://localhost:5000/api/voucher/all");
            
            if (response.IsSuccessStatusCode)
            {
                var vouchers = await response.Content.ReadFromJsonAsync<List<Voucher>>();
                return View("Vouchers/Vouchers", vouchers);
            }

            return View("Vouchers/Vouchers", new List<Voucher>());
        }

        public IActionResult CreateVoucher()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            return View("Vouchers/CreateVoucher");
        }

        [HttpPost]
        public async Task<IActionResult> CreateVoucher(Voucher voucher)
        {
            if (!IsAdmin()) return Unauthorized();

            var client = _httpClientFactory.CreateClient();
            var response = await client.PostAsJsonAsync("http://localhost:5000/api/voucher", voucher);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Thêm voucher thành công!";
                return RedirectToAction(nameof(Vouchers));
            }

            TempData["ErrorMessage"] = "Không thể thêm voucher!";
            return View(voucher);
        }

        public async Task<IActionResult> EditVoucher(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"http://localhost:5000/api/voucher/{id}");
            
            if (response.IsSuccessStatusCode)
            {
                var voucher = await response.Content.ReadFromJsonAsync<Voucher>();
                return View("Vouchers/EditVoucher", voucher);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> EditVoucher(int id, Voucher voucher)
        {
            if (!IsAdmin()) return Unauthorized();

            var client = _httpClientFactory.CreateClient();
            var response = await client.PutAsJsonAsync($"http://localhost:5000/api/voucher/{id}", voucher);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Cập nhật voucher thành công!";
                return RedirectToAction(nameof(Vouchers));
            }

            TempData["ErrorMessage"] = "Không thể cập nhật voucher!";
            return View("Vouchers/EditVoucher", voucher);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteVoucher(int id)
        {
            if (!IsAdmin()) return Unauthorized();

            var client = _httpClientFactory.CreateClient();
            var response = await client.DeleteAsync($"http://localhost:5000/api/voucher/{id}");

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Xóa voucher thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể xóa voucher!";
            }

            return RedirectToAction(nameof(Vouchers));
        }
    }

    // Dashboard stats model
    public class DashboardStats
    {
        public int TotalUsers { get; set; }
        public int TotalOrders { get; set; }
        public int TotalBeverages { get; set; }
        public decimal TotalRevenue { get; set; }
        public int PendingOrders { get; set; }
        public int LowStockItems { get; set; }
        public List<Order> RecentOrders { get; set; } = new();
        public Dictionary<string, int> OrdersByStatus { get; set; } = new();
    }
}
