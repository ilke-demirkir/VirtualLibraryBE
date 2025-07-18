namespace VirtualLibraryAPI.Dtos;

public class RegisterResponseDto
{
    public long Id { get; set; }
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    
    public DateTime CreatedAt { get; set; }
}
