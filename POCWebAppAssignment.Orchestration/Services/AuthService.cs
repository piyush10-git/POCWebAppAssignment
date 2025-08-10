using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using POCWebAppAssignment.Interfaces.Authentication;
using POCWebAppAssignment.Model.AuthDTOs;
using POCWebAppAssignment.Orchestration.HelperClasses;

namespace POCWebAppAssignment.Orchestration.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepo;
        private readonly IJWTService _jwtService;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IAuthRepository authRepo, IJWTService jwtService, ILogger<AuthService> logger)
        {
            _authRepo = authRepo;
            _jwtService = jwtService;
            _logger = logger;
        }

        public async Task<int?> SignUpAsync(SignupDto signup)
        {
            try
            {
                _logger.LogInformation("Starting user sign-up for username: {Username}", signup.UserName);

                // Hash password
                signup.Password = _jwtService.HashPassword(signup.Password);

                // Create user
                var userId = await _authRepo.CreateUserAsync(signup);

                if (userId == null)
                {
                    _logger.LogWarning("User creation failed for username: {Username}", signup.UserName);
                }
                else
                {
                    _logger.LogInformation("User created successfully with ID: {UserId} for username: {Username}", userId, signup.UserName);
                }

                return userId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred during sign-up for username: {Username}", signup.UserName);
                throw; // Let controller handle exception
            }
        }

        public async Task<LoginResultDto> LoginAsync(LoginDto loginCredentials)
        {
            try
            {
                _logger.LogInformation("Login attempt started for user/email: {UserInput}", loginCredentials.usernameOrEmail);

                // Get user by username or email
                var user = await _authRepo.GetUserWithRolesAsync(loginCredentials.usernameOrEmail);

                if (user == null)
                {
                    _logger.LogWarning("Login failed. No user found for: {UserInput}", loginCredentials.usernameOrEmail);
                    return new LoginResultDto(false, null, "Invalid username or password", null);
                }

                var hasher = new PasswordHasher<ApplicationUser>();
                var result = hasher.VerifyHashedPassword(
                    new ApplicationUser(), // should ideally pass actual user object
                    user.PasswordHash,
                    loginCredentials.Password
                );

                if (result != PasswordVerificationResult.Success)
                {
                    _logger.LogWarning("Invalid password attempt for user: {Username}", user.UserName);
                    return new LoginResultDto(false, null, "Invalid username or password", null);
                }

                // Generate token
                var token = _jwtService.GenerateJwtToken(user);
                _logger.LogInformation("Login successful for user: {Username}", user.UserName);

                return new LoginResultDto(true, token, null, user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception during login for: {UserInput}", loginCredentials.usernameOrEmail);
                throw; // Bubble up to controller
            }
        }
    }
}
