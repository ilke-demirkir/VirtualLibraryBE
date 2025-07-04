using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VirtualLibraryAPI.Data;
using VirtualLibraryAPI.Dtos;
using VirtualLibraryAPI.Entities;

namespace VirtualLibraryAPI.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly LibraryDbContext _ctx;
        private readonly IMapper           _mapper;
        private readonly CartService _cartService;

        public CartController(LibraryDbContext ctx, IMapper mapper, CartService cartService)
        {
            _ctx    = ctx;
            _mapper = mapper;
            _cartService = cartService;
        }

        // Helper to get current user
        private long CurrentUserId =>
            long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        // GET /api/cart
        [HttpGet]
        public async Task<ActionResult<List<CartItemDto>>> Get()
        {
            var items = await _ctx.CartItems
                .AsNoTracking()
                .Where(ci => ci.UserId == CurrentUserId)
                .Include(ci => ci.Book) // needed for Book.Name/Price projection
                .ProjectTo<CartItemDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Ok(items);
        }

        // POST /api/cart
        [HttpPost]
        public async Task<ActionResult<CartItemDto>> Post([FromBody] AddCartItemDto dto)
        {
            var existing = await _ctx.CartItems
                .FirstOrDefaultAsync(ci => ci.UserId == CurrentUserId && ci.BookId == dto.BookId);

            if (existing is null)
            {
                var entity = _mapper.Map<CartItem>(dto);
                entity.UserId = CurrentUserId;
                _ctx.CartItems.Add(entity);
                await _ctx.SaveChangesAsync();
                existing = entity;
            }
            else
            {
                existing.Quantity += dto.Quantity;
                await _ctx.SaveChangesAsync();
            }

            // return the up-to-date DTO
            var result = _mapper.Map<CartItemDto>(
                await _ctx.CartItems
                    .Include(ci => ci.Book)
                    .FirstAsync(ci => ci.Id == existing.Id)
            );
            return CreatedAtAction(nameof(Get), result);
        }

        // PATCH /api/cart/{id}
        [HttpPatch("{id}")]
        public async Task<ActionResult<CartItemDto>> Patch(int id, [FromBody] UpdateCartItemDto dto)
        {
            var item = await _ctx.CartItems
                .Include(ci => ci.Book)
                .FirstOrDefaultAsync(ci => ci.Id == id && ci.UserId == CurrentUserId);

            if (item is null) return NotFound();

            _mapper.Map(dto, item);
            await _ctx.SaveChangesAsync();

            return Ok(_mapper.Map<CartItemDto>(item));
        }

        // DELETE /api/cart/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _ctx.CartItems
                .FirstOrDefaultAsync(ci => ci.Id == id && ci.UserId == CurrentUserId);

            if (item is null) return NotFound();

            _ctx.CartItems.Remove(item);
            await _ctx.SaveChangesAsync();
            return NoContent();
        }
        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout()
        {
            // extract the current user
            var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            // call your CartService (or OrderService) to finalize the purchase
            await _cartService.CheckoutAsync(userId);

            return NoContent();
        }
    }
}
