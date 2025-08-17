using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using POCWebAppAssignment.Interfaces.Authentication;
using POCWebAppAssignment.Model.AuthDTOs;

namespace POCWebAppAssignment.Controllers
{
    [ApiController]
    [Route("api/admin")]
    //[Authorize(Roles = "Admin")] // only admins can access
    public class AdminController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AdminController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Create new user (provision)
        /// </summary>
        [HttpPost("create-user")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
        {
            var userId = await _authService.SignUpAsync(dto);
            if (userId == null) return BadRequest("Failed to create user.");

            return Ok(new { UserId = userId });
        }

        /// <summary>
        /// Provision temporary access (regenerate credentials)
        /// </summary>
        [HttpPost("provision/{userId}")]
        public async Task<IActionResult> ProvisionAccess(int userId, [FromQuery] int expiryHours = 24)
        {
            // Generate random temporary password (simple example)
            var tempPassword = Guid.NewGuid().ToString("N").Substring(0, 10) + "!";

            var success = await _authService.ProvisionAccessAsync(userId, tempPassword, TimeSpan.FromHours(expiryHours));
            if (!success) return BadRequest("Failed to provision credentials.");

            // In production: send email/SMS with tempPassword here
            return Ok(new { Message = "Temporary credentials created.", TempPassword = tempPassword });
        }

        /// <summary>
        /// Revoke user access
        /// </summary>
        [HttpPost("revoke/{userId}")]
        public async Task<IActionResult> RevokeAccess(int userId)
        {
            await _authService.RevokeAccessAsync(userId);
            return Ok("Access revoked successfully.");
        }
    }
}
