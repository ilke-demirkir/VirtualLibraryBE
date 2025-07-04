using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VirtualLibraryAPI.Data;
using VirtualLibraryAPI.Dtos;
using VirtualLibraryAPI.Entities;
using VirtualLibraryAPI.Services;

namespace VirtualLibraryAPI.Controllers
{
    public class FavoriteDto
    {
        public bool Fav { get; set; }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly LibraryDbContext _context;
        private readonly IMapper           _mapper;
        private readonly BookService _bookService;

        public BooksController(LibraryDbContext context, IMapper mapper, BookService bookService)
        {
            _context = context;
            _mapper  = mapper;
            _bookService = bookService;
        }

        // GET /api/books
        [HttpGet]
        public async Task<ActionResult<PagedResult<BookDto>>> GetAll(
            [FromQuery] string? title,
            [FromQuery] bool? featured,
            [FromQuery] string[] authors = null,
            [FromQuery] string[] categories = null,
            [FromQuery] int[] years = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 12
           
                )
        {
            
            var query = _context.Books.AsNoTracking().AsQueryable();
           
            if (!string.IsNullOrEmpty(title))
            {
                query = query.Where(b => EF.Functions.ILike(b.Name, $"%{title}%"));
            }

            
            if (authors != null && authors.Any())
            {
                query = query.Where(book => authors.Contains(book.Author ?? ""));
            }

            if (categories != null && categories.Any())
            {
                query = query.Where(book => categories.Contains(book.Category ?? ""));
            }

            if (years != null && years.Any())
            {
                query = query.Where(book => years.Contains(book.PublishYear ?? 0));
            }

            if (featured.HasValue)
            {
                query = query.Where(book => book.Featured == featured.Value);
            }
            
            
            var total = await query.CountAsync();
            
            var items = await query
                .OrderBy(b => b.Name)  // or whatever your default sort is
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<BookDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            // 5) wrap in PagedResult and return
            return Ok(new PagedResult<BookDto>
            {
                Items      = items,
                TotalCount = total
            });
        }

        // GET /api/books/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<BookDto>> GetById(int id)
        {
            var dto = await _context.Books
                .AsNoTracking()
                .Where(b => b.Id == id)
                .ProjectTo<BookDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            return dto is null ? NotFound() : Ok(dto);
        }

        // POST /api/books      (Admin only)
        [HttpPost, Authorize(Roles = "Admin")]
        public async Task<ActionResult<BookDto>> Create([FromBody] BookDto input)
        {
            var book = _mapper.Map<Book>(input);
            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            var dto = _mapper.Map<BookDto>(book);
            return CreatedAtAction(nameof(GetById), new { id = book.Id }, dto);
        }

        // PUT /api/books/{id}  (Admin only)
        [HttpPut("{id}"), Authorize(Roles = "Admin")]
        public async Task<ActionResult<BookDto>> Update(int id, [FromBody] BookDto input)
        {
            if (id != input.Id)
                return BadRequest("ID in URL and payload must match.");

            var book = await _context.Books.FindAsync(id);
            if (book is null)
                return NotFound();

            _mapper.Map(input, book);
            book.LastUpdate = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            var dto = _mapper.Map<BookDto>(book);
            return Ok(dto);
        }

        // DELETE /api/books/{id}  (Admin only)
        [HttpDelete("{id}"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book is null)
                return NotFound();

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // PATCH /api/books/{id}  (toggle favorite)
        [HttpPatch("{id}")]
        public async Task<ActionResult<BookDto>> UpdateFavorite(int id, [FromBody] FavoriteDto dto)
        {
            var book = await _context.Books.FindAsync(id);
            if (book is null)
                return NotFound();

            book.Fav = dto.Fav;
            await _context.SaveChangesAsync();

            var result = _mapper.Map<BookDto>(book);
            return Ok(result);
        }
        
        
        
        [HttpPost("import")]
        public async Task<IActionResult> Import([FromQuery] string title)
        {
            await _bookService.ImportByTitleAsync(title);
            return NoContent();
        }

        // 1b) (Optional) Import by ISBN
        //     POST /api/books/import/isbn/9780143127796
       
        // 1c) Your existing GET endpoint
        //     GET /api/books?title=foundation
        [HttpGet("search")]
        public async Task<IActionResult> Get([FromQuery] string title)
        {
            var books = await _bookService.FindByTitleAsync(title);
            return Ok(books);
        }

        [HttpGet("popular")]
        public async Task<ActionResult<IEnumerable<CategoryCountDto>>> GetPopular(int limit = 6)
        {
            var popular = await _context.Books
                .GroupBy(b => b.Category)
                .Select(b => new CategoryCountDto()
                {
                    Category = b.Key,
                    Count = b.Count()
                })
                .OrderByDescending(c  => c.Count)
                .Take(limit)
                .ToListAsync();
            return Ok(popular);
        }
    }
}
