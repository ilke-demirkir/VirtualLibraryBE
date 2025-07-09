// Controllers/WishlistController.cs

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VirtualLibraryAPI.Services;
using VirtualLibraryAPI.Dtos;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class WishlistController : ControllerBase
{
    private readonly WishlistService _svc;
    public WishlistController(WishlistService svc) => _svc = svc;

    // GET /api/wishlist
    [HttpGet]
    public async Task<IEnumerable<WishlistItemDto>> Get()
    {
        var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var items = await _svc.GetByUserAsync(userId);
        return items.Select(w => new WishlistItemDto(
            w.Id, w.BookId, w.Date
        ));
    }

    // POST /api/wishlist
    [HttpPost]
    public async Task Add([FromBody] CreateWishlistItemDto dto)
    {
        var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        Console.WriteLine($"UserID: { userId } , bookId: {dto.bookId}");
        await _svc.AddToWishlistAsync(userId, dto.bookId);
    }

    // DELETE /api/wishlist/{bookId}
    [HttpDelete("{bookId:long}")]
    public async Task Remove(long bookId)
    {
        var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        await _svc.RemoveFromWishlistAsync(userId, bookId);
    }
}