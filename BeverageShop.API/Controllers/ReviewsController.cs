using Microsoft.AspNetCore.Mvc;
using BeverageShop.API.Models;
using BeverageShop.API.Data;

namespace BeverageShop.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewsController : ControllerBase
    {
        [HttpGet("beverage/{beverageId}")]
        public ActionResult<IEnumerable<Review>> GetReviewsByBeverage(int beverageId)
        {
            var reviews = BeverageShopData.GetReviewsByBeverageId(beverageId);
            return Ok(reviews);
        }

        [HttpPost]
        public ActionResult<Review> CreateReview(Review review)
        {
            if (review.Rating < 1 || review.Rating > 5)
            {
                return BadRequest(new { message = "Đánh giá phải từ 1 đến 5 sao" });
            }

            var beverage = BeverageShopData.GetBeverageById(review.BeverageId);
            if (beverage == null)
            {
                return NotFound(new { message = "Không tìm thấy đồ uống" });
            }

            var newReview = BeverageShopData.AddReview(review);
            return CreatedAtAction(nameof(GetReviewsByBeverage), new { beverageId = review.BeverageId }, newReview);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteReview(int id)
        {
            var result = BeverageShopData.DeleteReview(id);
            if (!result)
            {
                return NotFound(new { message = "Không tìm thấy đánh giá" });
            }
            return NoContent();
        }

        [HttpGet("stats/{beverageId}")]
        public ActionResult<object> GetReviewStats(int beverageId)
        {
            var reviews = BeverageShopData.GetReviewsByBeverageId(beverageId);
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
