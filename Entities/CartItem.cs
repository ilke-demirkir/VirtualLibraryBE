using System.Text.Json.Serialization;

namespace VirtualLibraryAPI.Entities;

public class CartItem
{
    public long Id {get; set;}
    public long UserId {get; set;}
    public long BookId {get; set;}
    public int Quantity {get; set;}
    
    public Book Book {get; set;} = null!;
    
    public User User {get; set;} = null!;
    
    public List<OrderItem> Items {get; set;} = null!;
    
}