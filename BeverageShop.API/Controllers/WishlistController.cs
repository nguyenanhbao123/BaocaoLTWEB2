using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BeverageShop.API.Data;
using BeverageShop.API.Models;

namespace BeverageShop.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WishlistController : ControllerBase
    {
        private readonly BeverageShopDbContext _context;

        public WishlistController(BeverageShopDbContext context)
        {
            _context = context;
        }

        // GET: api/wishlist/user/{userId}
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<Wishlist>>> GetUserWishlist(int userId)
        {
            var wishlist = await _context.Wishlists
                .Include(w => w.Beverage)
                .Where(w => w.UserId == userId)
                .OrderByDescending(w => w.AddedDate)
                .ToListAsync();

            return Ok(wishlist);
        }

        // POST: api/wishlist
        [HttpPost]
        public async Task<ActionResult<Wishlist>> AddToWishlist(Wishlist wishlist)
        {
            // Check if already exists
            var exists = await _context.Wishlists
                .AnyAsync(w => w.UserId == wishlist.UserId && w.BeverageId == wishlist.BeverageId);

            if (exists)
            {
                return BadRequest("Item already in wishlist");
            }

            wishlist.AddedDate = DateTime.Now;
            _context.Wishlists.Add(wishlist);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUserWishlist), new { userId = wishlist.UserId }, wishlist);
        }

        // DELETE: api/wishlist/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveFromWishlist(int id)
        {
            var wishlist = await _context.Wishlists.FindAsync(id);
            if (wishlist == null)
            {
                return NotFound();
            }

            _context.Wishlists.Remove(wishlist);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/wishlist/user/{userId}/beverage/{beverageId}
        [HttpDelete("user/{userId}/beverage/{beverageId}")]
        public async Task<IActionResult> RemoveByUserAndBeverage(int userId, int beverageId)
        {
            var wishlist = await _context.Wishlists
                .FirstOrDefaultAsync(w => w.UserId == userId && w.BeverageId == beverageId);

            if (wishlist == null)
            {
                return NotFound();
            }

            _context.Wishlists.Remove(wishlist);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/wishlist/check/{userId}/{beverageId}
        [HttpGet("check/{userId}/{beverageId}")]
        public async Task<ActionResult<bool>> CheckInWishlist(int userId, int beverageId)
        {
            var exists = await _context.Wishlists
                .AnyAsync(w => w.UserId == userId && w.BeverageId == beverageId);

            return Ok(exists);
        }
    }
}
