using VirtualLibraryAPI.Entities;

namespace VirtualLibraryAPI.Dtos;

public record NotificationDto(
    long Id ,
    long UserId ,
    long? BookId ,
    NotificationType Type ,
    DateTime TimeStamp ,
    string Message ,
    bool IsRead 
);