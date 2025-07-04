namespace VirtualLibraryAPI.Entities;

public class User
{
    public long Id { get; set; }
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string Email { get; set; } = null!;
    public bool IsAdmin { get; set; } = false;
    
    public List<CartItem> CartItems { get; set; } = new List<CartItem>();
    public List<Order> Orders { get; set; } = new List<Order>();
}