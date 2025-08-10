using Microsoft.AspNetCore.Mvc;
using POCWebAppAssignment.API.Utilities;
using POCWebAppAssignment.Interfaces.Authentication;
using POCWebAppAssignment.Model.AuthDTOs;

namespace POCWebAppAssignment.API.Controllers
{
    [Route("api/admin")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AdminController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("provision-user")]
        public async Task<IActionResult> CreateUser([FromBody] LoginDto login)
        {
            try
            {
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }


        [HttpPost("resend-creds")]
        public async Task<IActionResult> ResendCredentials([FromBody] SignupDto signup)
        {
            try
            {
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        [HttpDelete("user/{id}")]
        public async Task<IActionResult> RevokeUserAccess(int id)
        {
            try
            {
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        [HttpPatch("user-role")]
        public async Task<IActionResult> UpdateUserAccess([FromBody] int val)
        {
            try
            {
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }
    }
}