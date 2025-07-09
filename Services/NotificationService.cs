using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using VirtualLibraryAPI.Data;
using VirtualLibraryAPI.Entities;
using VirtualLibraryAPI.Dtos;
using VirtualLibraryAPI.NotifHub;

namespace VirtualLibraryAPI.Services;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
public class NotificationService
{
    private readonly LibraryDbContext _context;
    private readonly IHubContext<NotificationHub> _notificationHub;
    public NotificationService(LibraryDbContext context, IHubContext<NotificationHub> notificationHub)
    {
        _context = context;
        _notificationHub = notificationHub;
    }
    public async Task NotifyUsersAsync(
        long? bookId,
        NotificationType type,
        string message)
    {
        List<long> userIds;
        if(string.IsNullOrWhiteSpace(message))
            message = "No message provided";
        if (bookId != null)
        {
            userIds = await _context.Wishlists
                .Where(w => w.BookId == bookId)
                .Select(w => w.UserId)
                .Distinct()
                .ToListAsync();
        }
        else
        {
            userIds = await _context.Users.Where(u => u.IsAdmin == false).Select(u => u.Id).Distinct().ToListAsync();
        }
     

        var notifs = userIds.Select(uid => new Notification
        {
            UserId    = uid,
            BookId    = bookId,
            Type      = type,
            Message   = message,
            TimeStamp = DateTime.UtcNow
        }).ToList();

        _context.Notifications.AddRange(notifs);
        await _context.SaveChangesAsync();

        foreach (var notif in notifs)
        {
            await _notificationHub.Clients
                .User(notif.UserId.ToString())
                .SendAsync("ReceiveNotification", new
                {
                    notif.Id,
                    notif.BookId,
                    notif.Type,
                    notif.Message,
                    notif.TimeStamp,
                    notif.IsRead
                });
        }
    }


    public Task<IEnumerable<Notification>> GetUserNotificationsAsync(long userId)
    {
        return Task.FromResult<IEnumerable<Notification>>(
            _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.TimeStamp)
                .AsEnumerable()
                .ToList()
        );
    }

    public async Task PublicAnnouncementAsync(CreateNotificationDto dto)
    {
        await NotifyUsersAsync(
            bookId: null,
            type: NotificationType.PublicAnnouncement,
            message: dto.Message
        );
    }

    public async Task MarkAsReadAsync(long userId, long notificationId)
    {
        var notif = await _context.Notifications
            .FirstOrDefaultAsync(n => n.UserId == userId && n.Id == notificationId);
        if (notif != null && !notif.IsRead)
        {
            notif.IsRead = true;
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteNotificationAsync(long userId, long notificationId)
    {
        var notif = await _context.Notifications.FirstOrDefaultAsync(n => n.UserId == userId && n.Id == notificationId);
        if (notif != null)
        {
            _context.Notifications.Remove(notif);
            await _context.SaveChangesAsync();
        }
    }
    
    
}