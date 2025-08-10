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
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginDto login)
        {
            _logger.LogInformation("Login attempt started for user: {Username}", login.usernameOrEmail);

            try
            {
                LoginResultDto loginResult = await _authService.LoginAsync(login);

                if (!loginResult.IsValid)
                {
                    _logger.LogWarning("Login failed for user: {Username}. Reason: {Error}", login.usernameOrEmail, loginResult.Error ?? "Invalid credentials");

                    var unauthorizedResponse = new ApiResponse<string?>(false, loginResult.Error ?? "Invalid username or password", null);
                    return Unauthorized(unauthorizedResponse); // 401
                }

                var loginData = new
                {
                    token = loginResult.Token,
                    username = loginResult.User!.UserName,
                    role = loginResult.User.Roles
                };

                _logger.LogInformation("Login successful for user: {Username}", login.usernameOrEmail);

                var successResponse = new ApiResponse<object>(true, "Login successful", loginData);
                return Ok(successResponse); // 200
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception during login for user: {Username}", login.usernameOrEmail);

                var errorResponse = new ApiResponse<string?>(false, "An internal server error occurred during login.", null);
                return StatusCode(500, errorResponse); // 500
            }
        }

        [HttpPost("sign-up")]
        public async Task<IActionResult> SignUp([FromBody] SignupDto signup)
        {
            _logger.LogInformation("Signup attempt for user: {Username}", signup.UserName);

            try
            {
                var userId = await _authService.SignUpAsync(signup);

                if (userId == null)
                {
                    _logger.LogWarning("User creation failed for username: {Username}", signup.UserName);

                    var failureResponse = new ApiResponse<string?>(false, "User creation failed. Please try again.", null);
                    return BadRequest(failureResponse); // 400
                }

                _logger.LogInformation("User created successfully with ID: {UserId} for username: {Username}", userId, signup.UserName);

                var successResponse = new ApiResponse<int?>(true, "User created successfully", userId);
                return StatusCode(201, successResponse); // 201 Created
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception during signup for username: {Username}", signup.UserName);

                var errorResponse = new ApiResponse<string?>(false, "An internal server error occurred during signup.", null);
                return StatusCode(500, errorResponse); // 500
            }
        }
    }
}
