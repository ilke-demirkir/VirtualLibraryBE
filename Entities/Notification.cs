namespace VirtualLibraryAPI.Entities;
public enum NotificationType {WishlistDiscount, PublicAnnouncement, WishlistRestock}
public class Notification
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public long? BookId { get; set; }
    public NotificationType Type { get; set; }
    public DateTime TimeStamp { get; set; }
    public User User { get; set; }
    public Book Book { get; set; }
    public string Message { get; set; }
    public bool IsRead { get; set; }
    
    
}