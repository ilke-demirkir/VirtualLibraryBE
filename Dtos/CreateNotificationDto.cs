using VirtualLibraryAPI.Entities;
namespace VirtualLibraryAPI.Dtos;

public record CreateNotificationDto(
    string Message,
    NotificationType Type
);
 
