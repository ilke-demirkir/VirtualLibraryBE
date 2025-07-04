namespace VirtualLibraryAPI.Dtos;

public record CartItemDto (
    int Id, 
    int BookId, 
    string BookName, 
    decimal BookPrice, 
    int Quantity
    );