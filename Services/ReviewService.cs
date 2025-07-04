// Services/ReviewService.cs
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using VirtualLibraryAPI.Data;
using VirtualLibraryAPI.Dtos;
using VirtualLibraryAPI.Entities;

namespace VirtualLibraryAPI.Services
{
    public class ReviewService
    {
        private readonly LibraryDbContext      _ctx;
        private readonly IHttpContextAccessor  _http;

        public ReviewService(
            LibraryDbContext      ctx,
            IHttpContextAccessor  http)
        {
            _ctx  = ctx;
            _http = http;
        }

        private long CurrentUserId
        {
            get
            {
                var idClaim = _http.HttpContext!
                    .User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                return long.Parse(idClaim!);
            }
        }

        private string CurrentUserName =>
            _http.HttpContext!.User.Identity?.Name ?? "Unknown";

        public async Task<ReviewWithRatingDto> AddReviewAsync(int bookId, CreateReviewDto dto)
        {
            var userId = CurrentUserId;
            var userEntity = await _ctx.Users.FirstOrDefaultAsync(u => u.Id == userId);
            var username = userEntity?.Username ?? "Unknown";
            var review = new Review
            {
                BookId    = bookId,
                UserId    = CurrentUserId,
                UserName  = username,
                Rating    = dto.Rating,
                Comment   = dto.Comment,
                CreatedAt = DateTime.UtcNow
            };
            _ctx.Reviews.Add(review);

            // update the book's average
            await _ctx.SaveChangesAsync();

            var ratings = await _ctx.Reviews
                .Where(r => r.BookId == bookId)
                .Select(r => (decimal)r.Rating)
                .ToListAsync();
            var avg = ratings.Any() ? ratings.Average() : 0m;

            // 3) persist back onto the Book
            var book = await _ctx.Books.FindAsync(bookId)
                       ?? throw new KeyNotFoundException("Book not found");
            book.AverageRating = avg;
            await _ctx.SaveChangesAsync();

            // 4) return both the new review AND the updated average
            var reviewDto = new ReviewDto(
                review.Id, review.UserId, review.UserName,
                review.Rating, review.Comment, review.CreatedAt
            );
            return new ReviewWithRatingDto(reviewDto, avg);
        }

        public async Task<List<ReviewDto>> GetReviewsAsync(int bookId)
        {
            return await _ctx.Reviews
                .Where(r => r.BookId == bookId)
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new ReviewDto(
                    r.Id,
                    r.UserId,
                    r.UserName,
                    r.Rating,
                    r.Comment,
                    r.CreatedAt
                ))
                .ToListAsync();
        }
        
        // Services/ReviewService.cs
        public async Task<ReviewWithRatingDto> DeleteReviewAsync(int bookId, int reviewId)
        {
            // 1) load the review
            var review = await _ctx.Reviews
                .FirstOrDefaultAsync(r => r.Id == reviewId && r.BookId == bookId);
            if (review is null)
                throw new KeyNotFoundException("Review not found.");

            // 2) check permissions
            var uid     = CurrentUserId;
            var isAdmin = _http.HttpContext!.User.IsInRole("Admin");
            if (review.UserId != uid && !isAdmin)
                throw new UnauthorizedAccessException("Not allowed to delete this review.");

            // 3) remove it
            _ctx.Reviews.Remove(review);

            await _ctx.SaveChangesAsync();
            
            await _ctx.SaveChangesAsync();

            // 2) recompute the book’s average
            var ratings = await _ctx.Reviews
                .Where(r => r.BookId == bookId)
                .Select(r => (decimal)r.Rating)
                .ToListAsync();
            var avg = ratings.Any() ? ratings.Average() : 0m;

            // 3) persist back onto the Book
            var book = await _ctx.Books.FindAsync(bookId)
                       ?? throw new KeyNotFoundException("Book not found");
            book.AverageRating = avg;
            await _ctx.SaveChangesAsync();

            // 4) return both the new review AND the updated average
            var reviewDto = new ReviewDto(
                review.Id, review.UserId, review.UserName,
                review.Rating, review.Comment, review.CreatedAt
            );
            return new ReviewWithRatingDto(reviewDto, avg);
            
            
        }

    }
}
