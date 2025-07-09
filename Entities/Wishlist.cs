namespace VirtualLibraryAPI.Entities;

public class Wishlist
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public long BookId { get; set; }
    public DateTime Date { get; set; }
    public User User { get; set; }
    public Book Book { get; set; }
}