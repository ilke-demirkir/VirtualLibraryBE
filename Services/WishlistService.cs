using System.Runtime.InteropServices.JavaScript;
using Microsoft.EntityFrameworkCore;
using VirtualLibraryAPI.Data;
using VirtualLibraryAPI.Entities;

namespace VirtualLibraryAPI.Services;

public class WishlistService
{
    public readonly LibraryDbContext _context;

    public WishlistService(LibraryDbContext context)
    {
        _context = context;
    }

    public async Task AddToWishlistAsync(long userId, long bookId)
    {
        if (!await _context.Wishlists.AnyAsync(w => w.BookId == bookId && w.UserId == userId))
        {
            _context.Wishlists.Add(new Wishlist
            {
                BookId = bookId,
                UserId = userId,
                Date = DateTime.UtcNow,
            });
            await _context.SaveChangesAsync();
        }
    }

    public async Task RemoveFromWishlistAsync(long userId, long bookId)
    {
        var item = await _context.Wishlists.FirstOrDefaultAsync(w => w.BookId == bookId && w.UserId == userId);

        if (item != null)
        {
            _context.Wishlists.Remove(item);
            await _context.SaveChangesAsync();
        }
    }

    public Task<IEnumerable<Wishlist>> GetByUserAsync(long userId)
    {
        return Task.FromResult<IEnumerable<Wishlist>>(
            _context.Wishlists
                .Where(w => w.UserId == userId).AsNoTracking().ToList()
        );
    }
    
}