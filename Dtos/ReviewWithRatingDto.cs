namespace VirtualLibraryAPI.Dtos;
public record ReviewWithRatingDto(ReviewDto Review, decimal AverageRating);