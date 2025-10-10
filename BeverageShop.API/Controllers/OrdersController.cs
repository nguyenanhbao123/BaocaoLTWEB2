using Microsoft.AspNetCore.Mvc;
using BeverageShop.API.Models;
using BeverageShop.API.Data;

namespace BeverageShop.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        [HttpGet]
        public ActionResult<IEnumerable<Order>> GetOrders([FromQuery] int? userId = null)
        {
            if (userId.HasValue)
            {
                return Ok(BeverageShopData.GetOrdersByUserId(userId.Value));
            }
            return Ok(BeverageShopData.GetOrders());
        }

        [HttpGet("{id}")]
        public ActionResult<Order> GetOrder(int id)
        {
            var order = BeverageShopData.GetOrderById(id);
            if (order == null)
            {
                return NotFound(new { message = "Không tìm thấy đơn hàng" });
            }
            return Ok(order);
        }

        [HttpGet("user/{userId}")]
        public ActionResult<IEnumerable<Order>> GetOrdersByUser(int userId)
        {
            var orders = BeverageShopData.GetOrdersByUserId(userId);
            return Ok(orders);
        }

        [HttpPost]
        public ActionResult<Order> CreateOrder(Order order)
        {
            // Validate stock
            foreach (var item in order.OrderItems)
            {
                var beverage = BeverageShopData.GetBeverageById(item.BeverageId);
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
                var beverage = BeverageShopData.GetBeverageById(item.BeverageId);
                if (beverage != null)
                {
                    beverage.Stock -= item.Quantity;
                    if (beverage.Stock == 0)
                    {
                        beverage.IsAvailable = false;
                    }
                }
            }

            var newOrder = BeverageShopData.AddOrder(order);
            return CreatedAtAction(nameof(GetOrder), new { id = newOrder.Id }, newOrder);
        }
    }
}
