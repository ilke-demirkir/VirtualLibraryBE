namespace VirtualLibraryAPI.Dtos;

public record ReviewDto(
    int      Id,
    long     UserId,
    string   UserName,
    int      Rating,
    string?  Comment,
    DateTime CreatedAt
);