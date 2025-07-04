using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VirtualLibraryAPI.Dtos;
using VirtualLibraryAPI.Services;

namespace VirtualLibraryAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class IyziController : ControllerBase
    {
        private readonly IyziService _iyzi;
        public IyziController(IyziService iyzi) => _iyzi = iyzi;

        
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreatePaymentFormDto dto)
        {
            var html = await _iyzi.CreatePaymentFormAsync(dto.CallbackUrl);
            return Ok(new { htmlForm = html });
        }
        
        [HttpPost("callback")]
        [AllowAnonymous]  // Iyzico won't send a JWT; callback must be public
        public async Task<IActionResult> Callback(
            [FromQuery] int userId,
            [FromForm] string token)
        {
            await _iyzi.HandleCallbackAsync(token, userId);
            // redirect into your Angular success route
            return Redirect("http://localhost:4200/checkout-success");
        }
    }
}