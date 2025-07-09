using AutoMapper;
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
    private readonly IMapper _mapper;
    public UserService(LibraryDbContext context, TokenService tokenService, IMapper mapper)
    {
        _tokenService = tokenService;
        _context = context;
        _mapper = mapper;
        
    }

    public async Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto dto)
    {
        var user = new User{ Username= dto.Username, Email = dto.Email, Password = BCrypt.Net.BCrypt.HashPassword(dto.Password), IsAdmin = false, CreatedAt = DateTime.UtcNow};
       
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        
        return new RegisterResponseDto {Id = user.Id, Username = user.Username, Email = user.Email, CreatedAt = DateTime.UtcNow };
    }


    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto dto)
    {
        var user = await  _context.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);
        
        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
            throw new Exception("Invalid username or password");
        
        var token = _tokenService.GenerateToken(user.Id, user.IsAdmin);
        
        return new LoginResponseDto {Id = user.Id, Username = user.Username, Token = token};
    }


    public async Task<User> EditUserAsync(long userId, EditUserDto dto)
    {
        var user = _context.Users.FirstOrDefault(u => u.Id == userId);
        if (user == null)
            throw new Exception("User not found");
        _mapper.Map(dto, user);
        if (!string.IsNullOrEmpty(dto.Password))
            user.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        
        return user;
    }
}