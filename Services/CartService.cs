using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using VirtualLibraryAPI.Data;
using VirtualLibraryAPI.Entities;
public class CartService
{
    private readonly LibraryDbContext _ctx;
    private readonly IHttpContextAccessor _http;  // to get current user

    public CartService(LibraryDbContext ctx, IHttpContextAccessor http)
    {
        _ctx  = ctx;
        _http = http;
    }

    public int CurrentUserId =>
        int.Parse(_http.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    public async Task<List<CartItem>> GetCartAsync()
    {
        return await _ctx.CartItems
                         .Include(ci => ci.Book)
                         .Where(ci => ci.UserId == CurrentUserId)
                         .ToListAsync();
    }

    public async Task<CartItem> AddOrUpdateAsync(int bookId, int qty)
    {
        var userId = CurrentUserId;
        var item = await _ctx.CartItems
                             .SingleOrDefaultAsync(ci => ci.UserId == userId && ci.BookId == bookId);

        if (item == null)
        {
            item = new CartItem { UserId = userId, BookId = bookId, Quantity = qty };
            _ctx.CartItems.Add(item);
        }
        else
        {
            item.Quantity += qty;
        }

        await _ctx.SaveChangesAsync();
        return item;
    }

    public async Task<CartItem?> UpdateQuantityAsync(int itemId, int qty)
    {
        var userId = CurrentUserId;
        var item = await _ctx.CartItems
                             .SingleOrDefaultAsync(ci => ci.UserId == userId && ci.Id == itemId);
        if (item == null) return null;
        item.Quantity = qty;
        await _ctx.SaveChangesAsync();
        return item;
    }

    public async Task<bool> RemoveAsync(int itemId)
    {
        var userId = CurrentUserId;
        var item = await _ctx.CartItems
                             .SingleOrDefaultAsync(ci => ci.UserId == userId && ci.Id == itemId);
        if (item == null) return false;
        _ctx.CartItems.Remove(item);
        await _ctx.SaveChangesAsync();
        return true;
    }

    public async Task CheckoutAsync()
    {
        var userId = CurrentUserId;
        
        var items = await _ctx.CartItems.Include(ci => ci.Book).Where(ci => ci.UserId == userId).ToListAsync();

        foreach (var ci in items)
        {
            var book = ci.Book;
            
            book.Stock = book.Stock - ci.Quantity;
            _ctx.Books.Update(book);
            _ctx.CartItems.Remove(ci);
        }
        await _ctx.SaveChangesAsync();
    }
    public async Task CheckoutAsync(long userId)
    {
        var items = await _ctx.CartItems.Include(ci => ci.Book).Where(ci => ci.UserId == userId).ToListAsync();
        
        foreach (var ci in items)
        {
            var book = ci.Book;
            if (ci.Book.Price is null)
                throw new InvalidOperationException(
                    $"Cannot checkout: Book (ID {ci.Book.Id}) has no price set.");
            
            book.Stock = book.Stock - ci.Quantity;
            _ctx.Books.Update(book);
            _ctx.CartItems.Remove(ci);
        }
        
        var order = new Order {
            UserId    = userId,
            CreatedAt = DateTime.UtcNow,
            Total     = items.Sum(ci => ci.Book.Price!.Value * ci.Quantity),
            Items     = items.Select(ci => new OrderItem {
                BookId    = ci.Book.Id,
                Quantity  = ci.Quantity,
                UnitPrice = ci.Book.Price!.Value,
            }).ToList()
        };
        _ctx.Orders.Add(order);

        // 3) commit everything
        await _ctx.SaveChangesAsync();
    }
}
