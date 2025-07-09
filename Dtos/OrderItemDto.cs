namespace VirtualLibraryAPI.Dtos;
public record OrderItemDto(long OrderId,string BookName, int Quantity, decimal UnitPrice);
public record OrderDetailDto(long Id, DateTime CreatedAt, decimal Total, List<OrderItemDto> Items);