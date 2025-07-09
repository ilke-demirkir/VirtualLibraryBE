using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Xml;
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
            Console.WriteLine($"[GET] UserId={CurrentUserId}");

            var items = await _ctx.CartItems
                .AsNoTracking()
                .Where(ci => ci.UserId == CurrentUserId)
                .Include(ci => ci.Book) // needed for Book.Name/Price projection
                .ProjectTo<CartItemDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
            Console.WriteLine($"[GET] Cart items count: {items.Count}");
            return Ok(items);
        }

        // POST /api/cart
        [HttpPost]
        public async Task<ActionResult<CartItemDto>> Post([FromBody] AddCartItemDto dto)
        {
            var book = await _ctx.Books.FirstOrDefaultAsync(b => b.Id == dto.BookId);
            if (book == null)
                return NotFound("Book not found.");

            var existing = await _ctx.CartItems
                .FirstOrDefaultAsync(ci => ci.UserId == CurrentUserId && ci.BookId == dto.BookId);
            Console.WriteLine($"[POST] Add to cart called for UserId={CurrentUserId}, BookId={dto.BookId}, Qty={dto.Quantity}");
            int totalRequested = dto.Quantity;
            if (existing != null)
                totalRequested += existing.Quantity;

            if (totalRequested > book.Stock)
                return BadRequest($"Cannot add {dto.Quantity} items to cart. Only {book.Stock} in stock (you already have {existing?.Quantity ?? 0} in your cart).");

            if (existing == null)
            {
                var entity = _mapper.Map<CartItem>(dto);
                entity.UserId = CurrentUserId;
                _ctx.CartItems.Add(entity);
                await _ctx.SaveChangesAsync();
                // After saving, entity.Id will be set
                existing = entity;
            }
            else
            {
                existing.Quantity += dto.Quantity;
                await _ctx.SaveChangesAsync();
            }

            // Defensive: refetch the item to ensure it's in the DB
            var resultItem = await _ctx.CartItems
                .Include(ci => ci.Book)
                .FirstOrDefaultAsync(ci => ci.Id == existing.Id);

            if (resultItem == null)
                return StatusCode(500, "Failed to retrieve cart item after saving.");

            var result = _mapper.Map<CartItemDto>(resultItem);
            Console.WriteLine($"[POST] UserId={CurrentUserId}, BookId={dto.BookId}, Qty={dto.Quantity}");
            return Ok(result);
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
