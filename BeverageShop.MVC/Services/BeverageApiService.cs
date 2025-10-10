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
                var json = JsonSerializer.Serialize(order);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PostAsync("api/orders", content);
                response.EnsureSuccessStatusCode();
                
                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<Order>(responseContent, _jsonOptions);
            }
            catch (Exception)
            {
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
    }
}
