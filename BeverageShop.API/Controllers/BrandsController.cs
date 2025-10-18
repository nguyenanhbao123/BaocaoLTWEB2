using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BeverageShop.API.Data;

namespace BeverageShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandsController : ControllerBase
    {
        private readonly BeverageShopDbContext _context;

        public BrandsController(BeverageShopDbContext context)
        {
            _context = context;
        }

        // GET: api/brands - Lấy danh sách tất cả brands
        [HttpGet]
        public async Task<ActionResult<IEnumerable<string>>> GetBrands()
        {
            var brands = await _context.Beverages
                .Where(b => !string.IsNullOrEmpty(b.Brand))
                .Select(b => b.Brand)
                .Distinct()
                .OrderBy(b => b)
                .ToListAsync();

            return Ok(brands);
        }

        // GET: api/brands/with-count - Lấy brands với số lượng sản phẩm
        [HttpGet("with-count")]
        public async Task<ActionResult<IEnumerable<object>>> GetBrandsWithCount()
        {
            var brands = await _context.Beverages
                .Where(b => !string.IsNullOrEmpty(b.Brand))
                .GroupBy(b => b.Brand)
                .Select(g => new
                {
                    name = g.Key,
                    count = g.Count(),
                    totalStock = g.Sum(b => b.Stock)
                })
                .OrderBy(b => b.name)
                .ToListAsync();

            return Ok(brands);
        }

        // PUT: api/brands/rename - Đổi tên brand
        [HttpPut("rename")]
        public async Task<IActionResult> RenameBrand([FromBody] RenameBrandRequest request)
        {
            if (string.IsNullOrEmpty(request.OldName) || string.IsNullOrEmpty(request.NewName))
            {
                return BadRequest(new { message = "Tên brand không được để trống" });
            }

            var beverages = await _context.Beverages
                .Where(b => b.Brand == request.OldName)
                .ToListAsync();

            if (!beverages.Any())
            {
                return NotFound(new { message = $"Không tìm thấy brand '{request.OldName}'" });
            }

            foreach (var beverage in beverages)
            {
                beverage.Brand = request.NewName;
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = $"Đã đổi tên brand từ '{request.OldName}' sang '{request.NewName}'", count = beverages.Count });
        }

        // DELETE: api/brands/{name} - Xóa brand (set về empty)
        [HttpDelete("{name}")]
        public async Task<IActionResult> DeleteBrand(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest(new { message = "Tên brand không được để trống" });
            }

            var beverages = await _context.Beverages
                .Where(b => b.Brand == name)
                .ToListAsync();

            if (!beverages.Any())
            {
                return NotFound(new { message = $"Không tìm thấy brand '{name}'" });
            }

            foreach (var beverage in beverages)
            {
                beverage.Brand = string.Empty;
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = $"Đã xóa brand '{name}' khỏi {beverages.Count} sản phẩm" });
        }

        // POST: api/brands/assign - Gán brand cho sản phẩm
        [HttpPost("assign")]
        public async Task<IActionResult> AssignBrand([FromBody] AssignBrandRequest request)
        {
            if (request.BeverageIds == null || !request.BeverageIds.Any())
            {
                return BadRequest(new { message = "Danh sách sản phẩm không được để trống" });
            }

            var beverages = await _context.Beverages
                .Where(b => request.BeverageIds.Contains(b.Id))
                .ToListAsync();

            if (!beverages.Any())
            {
                return NotFound(new { message = "Không tìm thấy sản phẩm nào" });
            }

            foreach (var beverage in beverages)
            {
                beverage.Brand = request.BrandName ?? string.Empty;
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = $"Đã gán brand '{request.BrandName}' cho {beverages.Count} sản phẩm" });
        }
    }

    public class RenameBrandRequest
    {
        public string OldName { get; set; } = string.Empty;
        public string NewName { get; set; } = string.Empty;
    }

    public class AssignBrandRequest
    {
        public List<int> BeverageIds { get; set; } = new();
        public string? BrandName { get; set; }
    }
}
