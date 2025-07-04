// Controllers/ReviewsController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VirtualLibraryAPI.Dtos;
using VirtualLibraryAPI.Services;

namespace VirtualLibraryAPI.Controllers
{
    [ApiController]
    [Route("api/books/{bookId}/reviews")]
    [Authorize]
    public class ReviewsController : ControllerBase
    {
        private readonly ReviewService _reviews;
        public ReviewsController(ReviewService reviews)
            => _reviews = reviews;

        /// <summary>
        /// GET /api/books/{bookId}/reviews
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> List(int bookId)
        {
            var list = await _reviews.GetReviewsAsync(bookId);
            return Ok(list);
        }

        /// <summary>
        /// POST /api/books/{bookId}/reviews
        /// Body: { rating: number, comment?: string }
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ReviewWithRatingDto>> Create(int bookId, [FromBody] CreateReviewDto dto)
        {
            var result = await _reviews.AddReviewAsync(bookId, dto);
            return Ok(result);
        }
        
        [HttpDelete("{id}")]
        public async Task<ActionResult<ReviewWithRatingDto>> Delete(int bookId, int id)
        {
            var result = await _reviews.DeleteReviewAsync(bookId, id);
            return Ok(result);
        }
    }
}