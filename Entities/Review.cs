namespace VirtualLibraryAPI.Entities
{
    public class Review
    {
        public int      Id        { get; set; }
        public int      BookId    { get; set; }
        public Book     Book      { get; set; } = null!;
        public long      UserId    { get; set; }
        public string   UserName  { get; set; } = null!;   // denormalize for display
        public int      Rating    { get; set; }            // 1–5
        public string?  Comment   { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
