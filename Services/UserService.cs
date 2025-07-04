using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
using VirtualLibraryAPI.Data;
using VirtualLibraryAPI.Dtos;
using VirtualLibraryAPI.Entities;
namespace VirtualLibraryAPI.Services;

public class UserService
{
    private readonly LibraryDbContext _context;
    private readonly TokenService _tokenService;
    public UserService(LibraryDbContext context, TokenService tokenService)
    {
        _tokenService = tokenService;
        _context = context;
    }

    public async Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto dto)
    {
        var user = new User{ Username= dto.Username, Email = dto.Email, Password = BCrypt.Net.BCrypt.HashPassword(dto.Password), IsAdmin = false };
        
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        
        return new RegisterResponseDto {Id = user.Id, Username = user.Username, Email = user.Email };
    }


    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto dto)
    {
        var user = await  _context.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);
        
        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
            throw new Exception("Invalid username or password");
        
        var token = _tokenService.GenerateToken(user.Id, user.IsAdmin);
        
        return new LoginResponseDto {Id = user.Id, Username = user.Username, Token = token};
    }
    
}