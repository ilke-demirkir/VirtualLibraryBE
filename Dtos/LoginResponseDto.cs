namespace VirtualLibraryAPI.Dtos;

public class LoginResponseDto
{
    public long Id { get; set; }
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public bool IsAdmin { get; set; }
    public string Token { get; set; } = null!;
}