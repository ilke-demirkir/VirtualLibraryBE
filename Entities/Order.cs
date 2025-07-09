using Iyzipay.Model;

namespace VirtualLibraryAPI.Entities;

public class Order
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public decimal Total { get; set; }
    
    public List<OrderItem> Items { get; set; }
    public User User { get; set; } = null!;
}