using System.Text.Json.Serialization;

namespace VirtualLibraryAPI.Entities;
public class OrderItem
{
    public long Id { get; set; }
    public long OrderId { get; set; }
    public Order Order { get; set; } = null!;
    public long BookId { get; set; }
    public Book Book { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}