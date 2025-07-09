using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using VirtualLibraryAPI.Entities;
using VirtualLibraryAPI.Data;
using VirtualLibraryAPI.NotifHub;
namespace VirtualLibraryAPI.Services
{
    public class BookService
    {
        private readonly LibraryDbContext _context;
        private readonly GoogleBooksService _googleBooksService;
        private readonly IHubContext<NotificationHub> _notificationHub;

        public BookService(LibraryDbContext context, GoogleBooksService googleBooksService)
        {
            _context = context;
            _googleBooksService = googleBooksService;
            
        }

        public List<Book> GetAll()
        {
            return _context.Books.ToList();
        }

        public Book? GetById(int id)
        {
            return _context.Books.Find(id);
        }
      
        public void Add(Book book)
        {
            _context.Books.Add(book);
            _context.SaveChanges(); 
        }
        public void Update(Book book)
        {
            _context.Books.Update(book);
            _context.SaveChanges();
        }
        
        public void Delete(long id)
        {
            var book = _context.Books.Find(id);
            if (book != null)
            {
                _context.Books.Remove(book);
                _context.SaveChanges();
            }
        }
        public async Task ImportByTitleAsync(string title)
        {
            // 1) Fetch the raw Book entities from GoogleBooksService
            var books = await _googleBooksService.SearchAndImportAsync(title);

            foreach (var book in books)
            {
                // 2) If we have an ISBN, skip any existing record with same ISBN
                if (!string.IsNullOrWhiteSpace(book.Isbn))
                {
                    var exists = await _context.Books.AnyAsync(b => b.Isbn == book.Isbn);
                    if (exists) continue;
                }
                // 3) Otherwise as a fallback, skip on Name+Year
                else if (await _context.Books.AnyAsync(b =>
                             b.Name == book.Name &&
                             b.PublishYear == book.PublishYear))
                {
                    continue;
                }

                // 4) New book—add to context
                _context.Books.Add(book);
            }

            // 5) Persist all at once
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Returns all books, or filters by title if provided.
        /// </summary>
        public async Task<PagedResult<Book>> GetAllAsync(string? title = null, int page = 1
        , int pageSize = 12)
        {
            var q = _context.Books.AsQueryable();
            if (!string.IsNullOrWhiteSpace(title))
                q = q.Where(b => EF.Functions.ILike(b.Name, $"%{title}%"));
            
            var total= await q.CountAsync();
            var items = await q
                .OrderBy(b => b.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            
            return new PagedResult<Book> {Items = items,TotalCount = total};
        }
        
        public async Task<IEnumerable<Book>> FindByTitleAsync(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                // return all if no filter
                return await _context.Books.ToListAsync();
            }

            return await _context.Books
                .Where(b => EF.Functions.ILike(b.Name, $"%{title}%"))
                .ToListAsync();
        }
        
        public Task<bool> HasAnyAsync()
            => _context.Books.AnyAsync();
    }
  

    
}