using Microsoft.AspNetCore.Mvc;
using BeverageShop.API.Models;
using BeverageShop.API.Data;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace BeverageShop.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly BeverageShopDbContext _context;

        public AuthController(BeverageShopDbContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<ActionResult<LoginResponse>> Register(RegisterRequest request)
        {
            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest(new LoginResponse 
                { 
                    Success = false, 
                    Message = "Tên đăng nhập và mật khẩu không được để trống" 
                });
            }

            // Kiểm tra username đã tồn tại trong database
            if (await _context.Users.AnyAsync(u => u.Username == request.Username))
            {
                return BadRequest(new LoginResponse 
                { 
                    Success = false, 
                    Message = "Tên đăng nhập đã tồn tại" 
                });
            }

            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = HashPassword(request.Password),
                FullName = request.FullName,
                Phone = request.Phone,
                Address = request.Address,
                Role = UserRole.Customer,
                IsActive = true,
                CreatedDate = DateTime.Now
            };

            // Lưu vào database
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new LoginResponse
            {
                Success = true,
                Message = "Đăng ký thành công",
                User = new User 
                { 
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    FullName = user.FullName,
                    Phone = user.Phone,
                    Address = user.Address,
                    Role = user.Role
                },
                Token = GenerateToken(user)
            });
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login(LoginRequest request)
        {
            // Tìm user trong database
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
            
            if (user == null || !VerifyPassword(request.Password, user.PasswordHash))
            {
                return BadRequest(new LoginResponse 
                { 
                    Success = false, 
                    Message = "Tên đăng nhập hoặc mật khẩu không đúng" 
                });
            }

            if (!user.IsActive)
            {
                return BadRequest(new LoginResponse 
                { 
                    Success = false, 
                    Message = "Tài khoản đã bị khóa" 
                });
            }

            return Ok(new LoginResponse
            {
                Success = true,
                Message = "Đăng nhập thành công",
                User = new User 
                { 
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    FullName = user.FullName,
                    Phone = user.Phone,
                    Address = user.Address,
                    Role = user.Role
                },
                Token = GenerateToken(user)
            });
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        private bool VerifyPassword(string password, string hash)
        {
            var hashedPassword = HashPassword(password);
            return hashedPassword == hash;
        }

        private string GenerateToken(User user)
        {
            // Simple token generation - in production, use JWT
            return Convert.ToBase64String(Encoding.UTF8.GetBytes($"{user.Id}:{user.Username}:{DateTime.Now.Ticks}"));
        }
    }
}
