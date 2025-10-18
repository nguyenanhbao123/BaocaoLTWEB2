using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BeverageShop.API.Models;
using BeverageShop.API.Data;

namespace BeverageShop.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewsController : ControllerBase
    {
        private readonly BeverageShopDbContext _context;

        public ReviewsController(BeverageShopDbContext context)
        {
            _context = context;
        }

        [HttpGet("beverage/{beverageId}")]
        public async Task<ActionResult<IEnumerable<Review>>> GetReviewsByBeverage(int beverageId)
        {
            var reviews = await _context.Reviews
                .Where(r => r.BeverageId == beverageId)
                .OrderByDescending(r => r.CreatedDate)
                .ToListAsync();
            return Ok(reviews);
        }

        [HttpPost]
        public async Task<ActionResult<Review>> CreateReview(Review review)
        {
            if (review.Rating < 1 || review.Rating > 5)
            {
                return BadRequest(new { message = "Đánh giá phải từ 1 đến 5 sao" });
            }

            var beverage = await _context.Beverages.FindAsync(review.BeverageId);
            if (beverage == null)
            {
                return NotFound(new { message = "Không tìm thấy đồ uống" });
            }

            review.CreatedDate = DateTime.Now;
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
            
            return CreatedAtAction(nameof(GetReviewsByBeverage), new { beverageId = review.BeverageId }, review);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReview(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null)
            {
                return NotFound(new { message = "Không tìm thấy đánh giá" });
            }
            
            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("stats/{beverageId}")]
        public async Task<ActionResult<object>> GetReviewStats(int beverageId)
        {
            var reviews = await _context.Reviews
                .Where(r => r.BeverageId == beverageId)
                .ToListAsync();
                
            if (!reviews.Any())
            {
                return Ok(new 
                { 
                    averageRating = 0.0,
                    totalReviews = 0,
                    ratingDistribution = new Dictionary<int, int>()
                });
            }

            var averageRating = reviews.Average(r => r.Rating);
            var ratingDistribution = reviews.GroupBy(r => r.Rating)
                .ToDictionary(g => g.Key, g => g.Count());

            return Ok(new
            {
                averageRating = Math.Round(averageRating, 1),
                totalReviews = reviews.Count(),
                ratingDistribution
            });
        }
    }
}
