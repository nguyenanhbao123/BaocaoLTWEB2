using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BeverageShop.API.Data;
using BeverageShop.API.Models;

namespace BeverageShop.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VoucherController : ControllerBase
    {
        private readonly BeverageShopDbContext _context;

        public VoucherController(BeverageShopDbContext context)
        {
            _context = context;
        }

        // GET: api/voucher
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Voucher>>> GetVouchers()
        {
            return await _context.Vouchers
                .Where(v => v.IsActive)
                .OrderByDescending(v => v.StartDate)
                .ToListAsync();
        }

        // GET: api/voucher/all (Admin only - get all vouchers including inactive)
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<Voucher>>> GetAllVouchers()
        {
            return await _context.Vouchers
                .OrderByDescending(v => v.CreatedDate)
                .ToListAsync();
        }

        // GET: api/voucher/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Voucher>> GetVoucher(int id)
        {
            var voucher = await _context.Vouchers.FindAsync(id);

            if (voucher == null)
            {
                return NotFound();
            }

            return voucher;
        }

        // GET: api/voucher/validate/{code}
        [HttpGet("validate/{code}")]
        public async Task<ActionResult<object>> ValidateVoucher(string code, [FromQuery] decimal orderAmount)
        {
            var voucher = await _context.Vouchers
                .FirstOrDefaultAsync(v => v.Code.ToLower() == code.ToLower());

            if (voucher == null)
            {
                return NotFound(new { message = "Mã voucher không tồn tại" });
            }

            if (!voucher.IsActive)
            {
                return BadRequest(new { message = "Voucher đã bị vô hiệu hóa" });
            }

            var now = DateTime.Now;
            if (now < voucher.StartDate)
            {
                return BadRequest(new { message = "Voucher chưa có hiệu lực" });
            }

            if (now > voucher.EndDate)
            {
                return BadRequest(new { message = "Voucher đã hết hạn" });
            }

            if (voucher.MaxUsageCount > 0 && voucher.UsedCount >= voucher.MaxUsageCount)
            {
                return BadRequest(new { message = "Voucher đã hết lượt sử dụng" });
            }

            if (orderAmount < voucher.MinimumOrderAmount)
            {
                return BadRequest(new 
                { 
                    message = $"Đơn hàng tối thiểu {voucher.MinimumOrderAmount:N0}đ để sử dụng voucher này" 
                });
            }

            // Calculate discount
            decimal discount = 0;
            if (voucher.Type == DiscountType.Percentage)
            {
                discount = orderAmount * voucher.Value / 100;
            }
            else
            {
                discount = voucher.Value;
            }

            return Ok(new
            {
                valid = true,
                voucher = voucher,
                discount = discount,
                finalAmount = orderAmount - discount
            });
        }

        // POST: api/voucher/apply
        [HttpPost("apply")]
        public async Task<ActionResult> ApplyVoucher([FromBody] string code)
        {
            var voucher = await _context.Vouchers
                .FirstOrDefaultAsync(v => v.Code.ToLower() == code.ToLower());

            if (voucher == null)
            {
                return NotFound();
            }

            voucher.UsedCount++;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Voucher applied successfully" });
        }

        // POST: api/voucher (Admin only)
        [HttpPost]
        public async Task<ActionResult<Voucher>> CreateVoucher(Voucher voucher)
        {
            _context.Vouchers.Add(voucher);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetVoucher), new { id = voucher.Id }, voucher);
        }

        // PUT: api/voucher/{id} (Admin only)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVoucher(int id, Voucher voucher)
        {
            if (id != voucher.Id)
            {
                return BadRequest();
            }

            _context.Entry(voucher).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await VoucherExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/voucher/{id} (Admin only)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVoucher(int id)
        {
            var voucher = await _context.Vouchers.FindAsync(id);
            if (voucher == null)
            {
                return NotFound();
            }

            _context.Vouchers.Remove(voucher);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private async Task<bool> VoucherExists(int id)
        {
            return await _context.Vouchers.AnyAsync(e => e.Id == id);
        }
    }
}
