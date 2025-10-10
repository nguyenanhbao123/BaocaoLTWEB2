using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BeverageShop.API.Models;
using BeverageShop.API.Data;

namespace BeverageShop.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly BeverageShopDbContext _context;

        public AdminController(BeverageShopDbContext context)
        {
            _context = context;
        }

        [HttpGet("dashboard")]
        public async Task<ActionResult<object>> GetDashboard()
        {
            try
            {
                var totalUsers = await _context.Users.CountAsync();
                var totalBeverages = await _context.Beverages.CountAsync();
                
                // Try to get orders data, if table doesn't exist yet, return 0
                int totalOrders = 0;
                decimal totalRevenue = 0;
                int pendingOrders = 0;
                List<Order> recentOrders = new List<Order>();
                Dictionary<string, int> ordersByStatus = new Dictionary<string, int>();
                
                try
                {
                    totalOrders = await _context.Orders.CountAsync();
                    totalRevenue = await _context.Orders
                        .Where(o => o.Status == OrderStatus.Delivered)
                        .SumAsync(o => o.TotalAmount);
                    pendingOrders = await _context.Orders.CountAsync(o => o.Status == OrderStatus.Pending);
                    
                    recentOrders = await _context.Orders
                        .Include(o => o.OrderItems)
                        .OrderByDescending(o => o.OrderDate)
                        .Take(10)
                        .ToListAsync();

                    ordersByStatus = await _context.Orders
                        .GroupBy(o => o.Status)
                        .Select(g => new { Status = g.Key.ToString(), Count = g.Count() })
                        .ToDictionaryAsync(x => x.Status, x => x.Count);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Warning: Could not fetch orders data - {ex.Message}");
                }
                
                var lowStockItems = await _context.Beverages.CountAsync(b => b.Stock <= 10 && b.Stock > 0);

                return Ok(new
                {
                    totalUsers,
                    totalOrders,
                    totalBeverages,
                    totalRevenue,
                    pendingOrders,
                    lowStockItems,
                    recentOrders,
                    ordersByStatus
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Dashboard Error: {ex.Message}");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("users")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            var users = await _context.Users
                .OrderByDescending(u => u.CreatedDate)
                .ToListAsync();
            
            // Remove password hashes before sending
            var safeUsers = users.Select(u => new User
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                FullName = u.FullName,
                Phone = u.Phone,
                Address = u.Address,
                Role = u.Role,
                CreatedDate = u.CreatedDate,
                IsActive = u.IsActive
            });
            return Ok(safeUsers);
        }

        [HttpPut("users/{id}/role")]
        public async Task<IActionResult> UpdateUserRole(int id, [FromBody] UpdateRoleRequest request)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound(new { message = "Không tìm thấy người dùng" });

            // Protect system admin account
            if (user.Username == "admin")
            {
                return BadRequest(new { message = "⛔ Không thể thay đổi quyền tài khoản hệ thống!" });
            }

            // Check if trying to demote the last admin
            if (user.Role == UserRole.Admin && request.Role == "Customer")
            {
                var adminCount = await _context.Users.CountAsync(u => u.Role == UserRole.Admin);
                if (adminCount <= 1)
                {
                    return BadRequest(new { message = "⚠️ Phải có ít nhất 1 Admin trong hệ thống!" });
                }
            }

            user.Role = request.Role == "Admin" ? UserRole.Admin : UserRole.Customer;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Cập nhật quyền thành công" });
        }

        [HttpPut("users/{id}/toggle-status")]
        public async Task<IActionResult> ToggleUserStatus(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound(new { message = "Không tìm thấy người dùng" });

            user.IsActive = !user.IsActive;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Cập nhật trạng thái thành công", isActive = user.IsActive });
        }

        [HttpPut("users/{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] User updatedUser)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound(new { message = "Không tìm thấy người dùng" });

            // Protect system admin account - cannot change username or role
            if (user.Username == "admin")
            {
                // Only allow updating personal info, not role or active status
                user.Email = updatedUser.Email;
                user.FullName = updatedUser.FullName;
                user.Phone = updatedUser.Phone;
                user.Address = updatedUser.Address;
                // Keep original role and active status
            }
            else
            {
                user.Email = updatedUser.Email;
                user.FullName = updatedUser.FullName;
                user.Phone = updatedUser.Phone;
                user.Address = updatedUser.Address;
                user.Role = updatedUser.Role;
                user.IsActive = updatedUser.IsActive;
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Cập nhật người dùng thành công" });
        }

        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound(new { message = "Không tìm thấy người dùng" });

            // Protect system admin account
            if (user.Username == "admin")
            {
                return BadRequest(new { message = "⛔ Không thể xóa tài khoản hệ thống!" });
            }

            // Check if user has orders
            var hasOrders = await _context.Orders.AnyAsync(o => o.UserId == id);
            if (hasOrders)
            {
                return BadRequest(new { message = "Không thể xóa người dùng đã có đơn hàng!" });
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Xóa người dùng thành công" });
        }

        [HttpPut("orders/{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] OrderStatusUpdate update)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
                return NotFound(new { message = "Không tìm thấy đơn hàng" });

            order.Status = update.Status;
            if (update.Status == OrderStatus.Delivered && !order.IsPaid)
            {
                order.IsPaid = true;
                order.PaidDate = DateTime.Now;
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Cập nhật trạng thái thành công" });
        }

        [HttpGet("beverages/low-stock")]
        public async Task<ActionResult<IEnumerable<Beverage>>> GetLowStockBeverages()
        {
            var beverages = await _context.Beverages
                .Where(b => b.Stock <= 10 && b.Stock > 0)
                .OrderBy(b => b.Stock)
                .ToListAsync();
            return Ok(beverages);
        }

        [HttpGet("beverages/out-of-stock")]
        public async Task<ActionResult<IEnumerable<Beverage>>> GetOutOfStockBeverages()
        {
            var beverages = await _context.Beverages
                .Where(b => b.Stock == 0)
                .ToListAsync();
            return Ok(beverages);
        }
    }

    public class OrderStatusUpdate
    {
        public OrderStatus Status { get; set; }
    }

    public class UpdateRoleRequest
    {
        public string Role { get; set; } = string.Empty;
    }
}
