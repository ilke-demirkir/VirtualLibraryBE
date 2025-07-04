using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using VirtualLibraryAPI.Data;
using VirtualLibraryAPI.Dtos;

namespace VirtualLibraryAPI.Services;

public class OrderService
{
    private readonly LibraryDbContext _ctx;
    private readonly IHttpContextAccessor _http;

    public OrderService(LibraryDbContext ctx, IHttpContextAccessor http)
    {
        _ctx = ctx;
        _http = http;
    }
    private int CurrentUserId => int.Parse(_http.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    public async Task<List<OrderSummaryDto>> GetMyOrdersAsync()
    {
        var userId = CurrentUserId;
        return await _ctx.Orders.Where(o => o.UserId == userId).OrderByDescending(o => o.CreatedAt)
            .Select(o => new OrderSummaryDto(o.Id, o.CreatedAt, o.Total)).ToListAsync();
    }
    
    public async Task<OrderDetailDto?> GetOrderAsync(int orderId)
    {
        var uid = CurrentUserId;
        var order = await _ctx.Orders
            .Include(o => o.Items).ThenInclude(i => i.Book)
            .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == uid);
        if (order == null) return null;

        var items = order.Items
            .Select(i => new OrderItemDto(i.BookId, i.Book.Name, i.Quantity, i.UnitPrice))
            .ToList();

        return new OrderDetailDto(order.Id, order.CreatedAt, order.Total, items);
    }
    
}