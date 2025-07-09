using Iyzipay.Request.V2.Subscription;
using Microsoft.AspNetCore.Mvc;
using VirtualLibraryAPI.Dtos;
using VirtualLibraryAPI.Services;
using Microsoft.AspNetCore.Authorization;
using VirtualLibraryAPI.Data;

namespace VirtualLibraryAPI.Controllers;

[ApiController]
[Route("api")]
public class UserController : ControllerBase
{
    private readonly UserService _userService;
    private readonly LibraryDbContext  _libraryDbContext;
    public UserController(UserService userService, LibraryDbContext libraryDbContext)
    {
        _userService = userService;
        _libraryDbContext = libraryDbContext;
        
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto dto)
    {
        if (_libraryDbContext.Users.Any(u => u.Username == dto.Username))
        {
            return BadRequest("Username already exists");
        }

        if (_libraryDbContext.Users.Any(u => u.Email == dto.Email))
        {
            return BadRequest("Email already exists");
        }
        var result = await _userService.RegisterAsync(dto);

        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
    {
        try
        {
            var result = await _userService.LoginAsync(dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return Unauthorized(ex.Message);
        }
    }

    [HttpPatch("{id:long}")]
    public async Task<IActionResult> PatchUser(long id, [FromBody] EditUserDto dto)
    {
        var user = await _userService.EditUserAsync(id, dto);
            
        await _libraryDbContext.SaveChangesAsync();
        return Ok(user);
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetUser(long id)
    {
        var user = await _libraryDbContext.Users.FindAsync(id);
        if (user == null) return NotFound();
        return Ok(user);
    }

}

