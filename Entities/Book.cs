using Stripe;

namespace VirtualLibraryAPI.Entities;

public class Book
{
    public int Id {get; set;}
    public string Name { get; set;} = string.Empty;
    public int Year{get; set;}
    public bool Fav {get; set;}
    
    public string? Author {get; set;} 
    
    public string? Description {get; set;} 
    
    public string? Image {get; set;} 
    
    public int? PublishYear {get; set;}
    
    public DateTime? LastUpdate {get; set;}
    
    public List<string>? Tags {get; set;} = new List<string>();
    
    public decimal? Price {get; set;}
    public int? Discount {get; set;}
    public bool? IsBestseller {get; set;}
    public string? Category {get; set;}
    public string? Publisher {get; set;}
    public List<Review>? Reviews {get; set;} 
    public string? Language {get; set;} = "en";
    public int? Stock {get; set;}
    public decimal? AverageRating {get; set;}
    
    public string? Isbn {get; set;}
    
    
    public List<CartItem>    CartItems   { get; set; } = new();
    public List<OrderItem>   OrderItems  { get; set; } = new();
    
    public bool Featured {get; set;}
}