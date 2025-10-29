using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using BeverageShop.MVC.Models;

namespace BeverageShop.MVC.Services
{
    public class BeverageApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly JsonSerializerOptions _jsonOptions;

        public BeverageApiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<List<Beverage>> GetBeveragesAsync(string? category = null)
        {
            try
            {
                var url = string.IsNullOrEmpty(category) 
                    ? "api/beverages" 
                    : $"api/beverages?category={category}";
                
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<Beverage>>(content, _jsonOptions) ?? new List<Beverage>();
            }
            catch (Exception)
            {
                return new List<Beverage>();
            }
        }

        public async Task<Beverage?> GetBeverageByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/beverages/{id}");
                response.EnsureSuccessStatusCode();
                
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<Beverage>(content, _jsonOptions);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<List<Beverage>> SearchBeveragesAsync(string keyword)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/beverages/search?keyword={keyword}");
                response.EnsureSuccessStatusCode();
                
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<Beverage>>(content, _jsonOptions) ?? new List<Beverage>();
            }
            catch (Exception)
            {
                return new List<Beverage>();
            }
        }

        public async Task<List<Category>> GetCategoriesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/categories");
                response.EnsureSuccessStatusCode();
                
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<Category>>(content, _jsonOptions) ?? new List<Category>();
            }
            catch (Exception)
            {
                return new List<Category>();
            }
        }

        public async Task<Order?> CreateOrderAsync(Order order)
        {
            try
            {
                var json = JsonSerializer.Serialize(order, new JsonSerializerOptions 
                { 
                    WriteIndented = true 
                });
                Console.WriteLine($"Creating order with JSON: {json}");
                
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PostAsync("api/orders", content);
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"API Error: {response.StatusCode} - {errorContent}");
                    return null;
                }
                
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Order created successfully: {responseContent}");
                return JsonSerializer.Deserialize<Order>(responseContent, _jsonOptions);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in CreateOrderAsync: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return null;
            }
        }

        // Auth methods
        public async Task<LoginResponse?> LoginAsync(string username, string password)
        {
            try
            {
                var loginData = new { Username = username, Password = password };
                var json = JsonSerializer.Serialize(loginData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PostAsync("api/auth/login", content);
                var responseContent = await response.Content.ReadAsStringAsync();
                
                return JsonSerializer.Deserialize<LoginResponse>(responseContent, _jsonOptions);
            }
            catch (Exception)
            {
                return new LoginResponse { Success = false, Message = "Lỗi kết nối server" };
            }
        }

        public async Task<LoginResponse?> RegisterAsync(RegisterViewModel model)
        {
            try
            {
                var json = JsonSerializer.Serialize(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PostAsync("api/auth/register", content);
                var responseContent = await response.Content.ReadAsStringAsync();
                
                return JsonSerializer.Deserialize<LoginResponse>(responseContent, _jsonOptions);
            }
            catch (Exception)
            {
                return new LoginResponse { Success = false, Message = "Lỗi kết nối server" };
            }
        }

        // Review methods
        public async Task<List<Review>> GetReviewsAsync(int beverageId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/reviews/beverage/{beverageId}");
                response.EnsureSuccessStatusCode();
                
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<Review>>(content, _jsonOptions) ?? new List<Review>();
            }
            catch (Exception)
            {
                return new List<Review>();
            }
        }

        public async Task<ReviewStats?> GetReviewStatsAsync(int beverageId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/reviews/stats/{beverageId}");
                response.EnsureSuccessStatusCode();
                
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<ReviewStats>(content, _jsonOptions);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> CreateReviewAsync(Review review)
        {
            try
            {
                var json = JsonSerializer.Serialize(review);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("api/reviews", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // Wishlist methods
        public async Task<bool> AddToWishlistAsync(Wishlist wishlist)
        {
            try
            {
                var json = JsonSerializer.Serialize(wishlist);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("api/wishlist", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> RemoveFromWishlistAsync(int wishlistId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/wishlist/{wishlistId}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> IsInWishlistAsync(int userId, int beverageId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/wishlist/check/{userId}/{beverageId}");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<bool>(content, _jsonOptions);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> RemoveFromWishlistByUserAndBeverageAsync(int userId, int beverageId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/wishlist/user/{userId}/beverage/{beverageId}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<BeverageShop.MVC.Models.Wishlist>> GetUserWishlistAsync(int userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/wishlist/user/{userId}");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<BeverageShop.MVC.Models.Wishlist>>(content, _jsonOptions) ?? new List<BeverageShop.MVC.Models.Wishlist>();
            }
            catch (Exception)
            {
                return new List<BeverageShop.MVC.Models.Wishlist>();
            }
        }

        public async Task<BeverageShop.MVC.Models.Wishlist?> CreateWishlistAsync(BeverageShop.MVC.Models.Wishlist wishlist)
        {
            try
            {
                var json = JsonSerializer.Serialize(wishlist);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("api/wishlist", content);
                if (!response.IsSuccessStatusCode) return null;
                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<BeverageShop.MVC.Models.Wishlist>(responseContent, _jsonOptions);
            }
            catch (Exception)
            {
                return null;
            }
        }

        // Order tracking
        public async Task<List<Order>> GetUserOrdersAsync(int userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/orders/user/{userId}");
                response.EnsureSuccessStatusCode();
                
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<Order>>(content, _jsonOptions) ?? new List<Order>();
            }
            catch (Exception)
            {
                return new List<Order>();
            }
        }

        public async Task<Order?> GetOrderByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/orders/{id}");
                response.EnsureSuccessStatusCode();
                
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<Order>(content, _jsonOptions);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<Beverage?> CreateBeverageAsync(Beverage beverage)
        {
            try
            {
                var json = JsonSerializer.Serialize(beverage);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("api/beverages", content);

                if (!response.IsSuccessStatusCode)
                {
                    var err = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"CreateBeverageAsync failed: {response.StatusCode} - {err}");
                    return null;
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<Beverage>(responseContent, _jsonOptions);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in CreateBeverageAsync: {ex.Message}");
                return null;
            }
        }
    }
}
