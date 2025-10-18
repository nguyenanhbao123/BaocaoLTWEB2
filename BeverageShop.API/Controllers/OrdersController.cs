using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BeverageShop.API.Models;
using BeverageShop.API.Data;

namespace BeverageShop.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly BeverageShopDbContext _context;

        public OrdersController(BeverageShopDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders([FromQuery] int? userId = null)
        {
            if (userId.HasValue)
            {
                var userOrders = await _context.Orders
                    .Include(o => o.OrderItems)
                    .Where(o => o.UserId == userId.Value)
                    .OrderByDescending(o => o.OrderDate)
                    .ToListAsync();
                return Ok(userOrders);
            }
            
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id);
                
            if (order == null)
            {
                return NotFound(new { message = "Không tìm thấy đơn hàng" });
            }
            return Ok(order);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrdersByUser(int userId)
        {
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
            return Ok(orders);
        }

        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder(Order order)
        {
            // Validate stock
            foreach (var item in order.OrderItems)
            {
                var beverage = await _context.Beverages.FindAsync(item.BeverageId);
                if (beverage == null)
                {
                    return BadRequest(new { message = $"Không tìm thấy đồ uống ID {item.BeverageId}" });
                }
                if (beverage.Stock < item.Quantity)
                {
                    return BadRequest(new { message = $"Không đủ hàng cho {beverage.Name}" });
                }
            }

            // Update stock
            foreach (var item in order.OrderItems)
            {
                var beverage = await _context.Beverages.FindAsync(item.BeverageId);
                if (beverage != null)
                {
                    beverage.Stock -= item.Quantity;
                    if (beverage.Stock == 0)
                    {
                        beverage.IsAvailable = false;
                    }
                }
            }

            order.OrderDate = DateTime.Now;
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            
            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
        }
    }
}
