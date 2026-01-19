using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Application.DTOS.Review;
using Restaurant.Application.Services.ReviewSR;

namespace Restaurant.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService reviewService;
        public ReviewController(IReviewService reviewService)
        {
            this.reviewService = reviewService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllReviews()
        {
            var res = await reviewService.GetAllReviews();
            return Ok(res);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetReviewById(int id)
        {
            var res = await reviewService.GetReviewById(id);
            return Ok(res);
        }
        [HttpGet("user/{customerId}")]  // role for admin
        public async Task<IActionResult> GetAllReviewforUser(int customerId)
        {
            var res = await reviewService.GetAllReviewforUser(customerId);
            return Ok(res);
        }
        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> CreateReview([FromBody] CreateReviewDTO dto)
        {
            var userid=int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var res = await reviewService.CreateReview(dto, userid);
            return Ok(res);
        }
        [HttpPut("{reviewId}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> UpdateReview(int reviewId, [FromBody] CreateReviewDTO dto)
        {
            var userid = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var res = await reviewService.UpdateReview(dto, userid, reviewId);
            return Ok(res);
        }
        [HttpDelete("{reviewId}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> DeleteReview(int reviewId)
        {
            var userid = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var res = await reviewService.DeleteReview(reviewId, userid);
            if (!res)
                return BadRequest("Delete Failed");
            return Ok("Deleted Sussfully");
        }
    }
}
