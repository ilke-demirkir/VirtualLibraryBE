// Controllers/NotificationsController.cs

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VirtualLibraryAPI.Data;
using VirtualLibraryAPI.Services;
using VirtualLibraryAPI.Dtos;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly NotificationService _svc;

    public NotificationsController(NotificationService svc, LibraryDbContext context)
    {
        _svc = svc;
    }


    // GET /api/notifications
    [HttpGet]
    public async Task<IEnumerable<NotificationDto>> Get()
    {
        var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var notifs = await _svc.GetUserNotificationsAsync(userId);
        return notifs.Select(n => new NotificationDto(
            n.Id, n.UserId, n.BookId, n.Type,  n.TimeStamp, n.Message, n.IsRead
        ));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task PublicAnnouncementAsync(CreateNotificationDto dto)
    {
        await _svc.PublicAnnouncementAsync(dto);
    }
    
    // PUT /api/notifications/{id}/read
    [HttpPut("{id:long}/read")]
    public async Task MarkAsRead(long id)
    {
        var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        await _svc.MarkAsReadAsync(userId, id);
         
    }
    // DELETE /api/notifications/{id}
    [HttpDelete("{id:long}")]
    public async Task Delete(long id)
    {
        var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        await _svc.DeleteNotificationAsync(userId, id);
    }
}