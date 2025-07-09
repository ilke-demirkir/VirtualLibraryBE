namespace VirtualLibraryAPI.Dtos;

public record ReviewDto(
    long      Id,
    long     UserId,
    string   UserName,
    int      Rating,
    string?  Comment,
    DateTime CreatedAt
);