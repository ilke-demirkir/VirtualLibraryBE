namespace VirtualLibraryAPI.Dtos;

public class EditUserDto
{
    public string? AvatarUrl { get; set; }
    public string? Bio { get; set; }

    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
}