using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using POCWebAppAssignment.API.Utilities;
using POCWebAppAssignment.Interfaces.Authentication;
using POCWebAppAssignment.Model.AuthDTOs;

namespace POCWebAppAssignment.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var result = await _authService.LoginAsync(dto);

            ApiResponse<object?> response;
            if (!result.Success)
            {
                response = new ApiResponse<object?>(true, "log in Failed", null);
            } else
            {
                response = new ApiResponse<object?>(true, "log in Successful", new {token =  result.Token });
            }

            return result.Success ? Ok(response) : Unauthorized(response);
        }


        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            //var userId = int.Parse(User.FindFirst("UserId")!.Value);

            var success = await _authService.ChangePasswordAsync(dto.UserId, dto.OldPassword, dto.NewPassword);

            if (!success) return BadRequest("Old password is incorrect.");

            return Ok("Password updated successfully.");
        }
    }
}
