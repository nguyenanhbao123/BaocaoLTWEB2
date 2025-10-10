using Microsoft.AspNetCore.Mvc;
using BeverageShop.MVC.Models;
using BeverageShop.MVC.Services;
using System.Text.Json;

namespace BeverageShop.MVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly BeverageApiService _apiService;
        private const string UserSessionKey = "CurrentUser";

        public AccountController(BeverageApiService apiService)
        {
            _apiService = apiService;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var response = await _apiService.LoginAsync(model.Username, model.Password);
            
            if (response == null || !response.Success)
            {
                ModelState.AddModelError("", response?.Message ?? "Đăng nhập thất bại");
                return View(model);
            }

            // Save user to session
            HttpContext.Session.SetString(UserSessionKey, JsonSerializer.Serialize(response.User));
            TempData["SuccessMessage"] = "Đăng nhập thành công!";

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            // Redirect to Admin dashboard if admin, otherwise home
            if (response.User?.Role == UserRole.Admin)
            {
                return RedirectToAction("Index", "Admin");
            }
            
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.Password != model.ConfirmPassword)
            {
                ModelState.AddModelError("", "Mật khẩu xác nhận không khớp");
                return View(model);
            }

            var response = await _apiService.RegisterAsync(model);
            
            if (response == null || !response.Success)
            {
                ModelState.AddModelError("", response?.Message ?? "Đăng ký thất bại");
                return View(model);
            }

            // Auto login after register
            HttpContext.Session.SetString(UserSessionKey, JsonSerializer.Serialize(response.User));
            TempData["SuccessMessage"] = "Đăng ký thành công!";

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove(UserSessionKey);
            TempData["SuccessMessage"] = "Đăng xuất thành công!";
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Orders()
        {
            var userJson = HttpContext.Session.GetString(UserSessionKey);
            if (string.IsNullOrEmpty(userJson))
            {
                return RedirectToAction("Login", new { returnUrl = "/Account/Orders" });
            }

            var user = JsonSerializer.Deserialize<User>(userJson);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var orders = await _apiService.GetUserOrdersAsync(user.Id);
            return View(orders);
        }

        public async Task<IActionResult> OrderDetails(int id)
        {
            var userJson = HttpContext.Session.GetString(UserSessionKey);
            if (string.IsNullOrEmpty(userJson))
            {
                return RedirectToAction("Login", new { returnUrl = $"/Account/OrderDetails/{id}" });
            }

            var order = await _apiService.GetOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        public IActionResult Profile()
        {
            var userJson = HttpContext.Session.GetString(UserSessionKey);
            if (string.IsNullOrEmpty(userJson))
            {
                return RedirectToAction("Login", new { returnUrl = "/Account/Profile" });
            }

            var user = JsonSerializer.Deserialize<User>(userJson);
            return View(user);
        }
    }
}
