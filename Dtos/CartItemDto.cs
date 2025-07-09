namespace VirtualLibraryAPI.Dtos;

public record CartItemDto (
    long Id, 
    long BookId, 
    string BookName, 
    decimal BookPrice, 
    int Quantity
    );