using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BeverageShop.API.Models;
using BeverageShop.API.Data;

namespace BeverageShop.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BeveragesController : ControllerBase
    {
        private readonly BeverageShopDbContext _context;

        public BeveragesController(BeverageShopDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Beverage>>> GetBeverages([FromQuery] string? category = null)
        {
            var query = _context.Beverages.AsQueryable();
            
            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(b => b.Category == category);
            }
            
            var beverages = await query.ToListAsync();
            return Ok(beverages);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Beverage>> GetBeverage(int id)
        {
            var beverage = await _context.Beverages.FindAsync(id);
            if (beverage == null)
            {
                return NotFound(new { message = "Không tìm thấy đồ uống" });
            }
            return Ok(beverage);
        }

        [HttpPost]
        public async Task<ActionResult<Beverage>> CreateBeverage(Beverage beverage)
        {
            beverage.CreatedDate = DateTime.Now;
            _context.Beverages.Add(beverage);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetBeverage), new { id = beverage.Id }, beverage);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBeverage(int id, Beverage beverage)
        {
            if (id != beverage.Id)
            {
                return BadRequest(new { message = "ID không khớp" });
            }

            var existing = await _context.Beverages.FindAsync(id);
            if (existing == null)
            {
                return NotFound(new { message = "Không tìm thấy đồ uống" });
            }

            existing.Name = beverage.Name;
            existing.Type = beverage.Type;
            existing.Category = beverage.Category;
            existing.Price = beverage.Price;
            existing.Size = beverage.Size;
            existing.Description = beverage.Description;
            existing.ImageUrl = beverage.ImageUrl;
            existing.Stock = beverage.Stock;
            existing.IsAvailable = beverage.IsAvailable;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBeverage(int id)
        {
            var beverage = await _context.Beverages.FindAsync(id);
            if (beverage == null)
            {
                return NotFound(new { message = "Không tìm thấy đồ uống" });
            }

            _context.Beverages.Remove(beverage);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Beverage>>> SearchBeverages([FromQuery] string keyword)
        {
            var beverages = await _context.Beverages
                .Where(b => b.Name.Contains(keyword) ||
                           b.Type.Contains(keyword) ||
                           b.Description.Contains(keyword))
                .ToListAsync();

            return Ok(beverages);
        }

        // Advanced Search & Filter
        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<Beverage>>> FilterBeverages(
            [FromQuery] string? category = null,
            [FromQuery] string? type = null,
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null,
            [FromQuery] string? size = null,
            [FromQuery] string? sortBy = "name",
            [FromQuery] string? order = "asc",
            [FromQuery] string? keyword = null)
        {
            var query = _context.Beverages.AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(b => b.Category == category);
            }

            if (!string.IsNullOrEmpty(type))
            {
                query = query.Where(b => b.Type == type);
            }

            if (minPrice.HasValue)
            {
                query = query.Where(b => b.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(b => b.Price <= maxPrice.Value);
            }

            if (!string.IsNullOrEmpty(size))
            {
                query = query.Where(b => b.Size == size);
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(b => b.Name.Contains(keyword) ||
                                       b.Type.Contains(keyword) ||
                                       b.Description.Contains(keyword));
            }

            // Apply sorting
            query = sortBy?.ToLower() switch
            {
                "price" => order == "desc" ? query.OrderByDescending(b => b.Price) : query.OrderBy(b => b.Price),
                "date" => order == "desc" ? query.OrderByDescending(b => b.CreatedDate) : query.OrderBy(b => b.CreatedDate),
                _ => order == "desc" ? query.OrderByDescending(b => b.Name) : query.OrderBy(b => b.Name)
            };

            var beverages = await query.ToListAsync();
            return Ok(beverages);
        }
    }
}

