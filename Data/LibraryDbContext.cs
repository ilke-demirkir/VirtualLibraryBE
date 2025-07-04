using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using VirtualLibraryAPI.Entities;

namespace VirtualLibraryAPI.Data
{
    // Data/LibraryDbContext.cs
    // Data/LibraryDbContext.cs
    public class LibraryDbContext : DbContext
    {
        public LibraryDbContext(DbContextOptions<LibraryDbContext> options)
            : base(options) { }

        public DbSet<Book>       Books       { get; set; }
        public DbSet<User>       Users       { get; set; }
        public DbSet<CartItem>   CartItems   { get; set; }
        public DbSet<Order>      Orders      { get; set; }
        public DbSet<OrderItem>  OrderItems  { get; set; }
        public DbSet<Review>     Reviews     { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // 1) Reviews ← Book (1:M)
            builder.Entity<Review>()
                .HasOne(r => r.Book)
                .WithMany(b => b.Reviews)
                .HasForeignKey(r => r.BookId);

            // 2) CartItems ← User (1:M)
            builder.Entity<CartItem>()
                .HasOne(ci => ci.User)
                .WithMany(u => u.CartItems)
                .HasForeignKey(ci => ci.UserId);

            // 3) CartItems ← Book (1:M)
            builder.Entity<CartItem>()
                .HasOne(ci => ci.Book)
                .WithMany(b => b.CartItems)
                .HasForeignKey(ci => ci.BookId);

            // 4) OrderItems ← Order (1:M)
            builder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.Items)
                .HasForeignKey(oi => oi.OrderId);

            // 5) OrderItems ← Book (1:M)
            builder.Entity<OrderItem>()
                .HasOne(oi => oi.Book)
                .WithMany(b => b.OrderItems)
                .HasForeignKey(oi => oi.BookId);
        }
    }

}