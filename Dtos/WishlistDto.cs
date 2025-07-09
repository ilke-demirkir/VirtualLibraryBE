namespace VirtualLibraryAPI.Dtos;

public record WishlistItemDto(
    long Id,
    long BookId,
    DateTime Date);
   
public record CreateWishlistItemDto(long userId, long bookId);