namespace VirtualLibraryAPI.Dtos;
public record OrderItemDto(int OrderId,string BookName, int Quantity, decimal UnitPrice);
public record OrderDetailDto(int Id, DateTime CreatedAt, decimal Total, List<OrderItemDto> Items);