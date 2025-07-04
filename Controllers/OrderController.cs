using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VirtualLibraryAPI.Services;

namespace VirtualLibraryAPI.Controllers;

// Controllers/OrdersController.cs
[ApiController]
[Authorize]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly OrderService _orders;
    public OrdersController(OrderService orders) => _orders = orders;

    [HttpGet]
    public async Task<IActionResult> List()
    {
        var list = await _orders.GetMyOrdersAsync();
        return Ok(list);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var dto = await _orders.GetOrderAsync(id);
        return dto is not null ? Ok(dto) : NotFound();
    }
}
