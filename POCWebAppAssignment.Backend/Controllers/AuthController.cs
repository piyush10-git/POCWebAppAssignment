using Microsoft.AspNetCore.Mvc;
using POCWebAppAssignment.API.Utilities;
using POCWebAppAssignment.Interfaces.Authentication;
using POCWebAppAssignment.Model.AuthDTOs;

namespace POCWebAppAssignment.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Post([FromBody] LoginDto login)
        {
            try
            {
                LoginResultDto loginResult = await _authService.LoginAsync(login);

                if (!loginResult.IsValid)
                {
                    var unauthorizedResponse = new ApiResponse<string?>(false, loginResult.Error ?? "Invalid credentials", null);
                    return Unauthorized(unauthorizedResponse);
                }

                var loginData = new
                {
                    token = loginResult.Token,
                    username = loginResult.User!.UserName,
                    role = loginResult.User.Roles
                };

                var successResponse = new ApiResponse<object>(true, "Login successful", loginData);
                return Ok(successResponse);
            }
            catch (Exception ex)
            {
                var errorResponse = new ApiResponse<string?>(false, "An error occurred during login", ex.Message);
                return StatusCode(500, errorResponse);
            }
        }


        [HttpPost("sign-up")]
        public async Task<IActionResult> SignUp([FromBody] SignupDto signup)
        {
            try
            {
                var userId = await _authService.SignUpAsync(signup);

                if (userId == null)
                {
                    var failureResponse = new ApiResponse<string?>(false, "User creation failed", null);
                    return BadRequest(failureResponse);
                }

                var successResponse = new ApiResponse<int?>(true, "User created successfully", userId);
                return Ok(successResponse);
            }
            catch (Exception ex)
            {
                var errorResponse = new ApiResponse<string?>(false, "Something went wrong", ex.Message);
                return StatusCode(500, errorResponse);
            }
        }
    }
}