using System.Runtime.InteropServices.JavaScript;

namespace VirtualLibraryAPI.Entities;

public class User
{
    public long Id { get; set; }
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string Email { get; set; } = null!;
    public bool IsAdmin { get; set; } = false;
    
    public string? AvatarUrl { get; set; } = null!;
    public string? Bio { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    public List<CartItem> CartItems { get; set; } = new List<CartItem>();
    public List<Order> Orders { get; set; } = new List<Order>();
    public List<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
    public List<Notification> Notifications { get; set; } = new List<Notification>();
}