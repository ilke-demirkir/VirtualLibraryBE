using Microsoft.EntityFrameworkCore;
using VirtualLibraryAPI.Models;

namespace VirtualLibraryAPI.Data
{
    public class LibraryDbContext : DbContext
    {
        public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options) {}
        
        public DbSet<Book> Books { get; set; }
    }
}

