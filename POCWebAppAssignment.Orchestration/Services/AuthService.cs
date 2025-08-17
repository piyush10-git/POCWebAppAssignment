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

        public AuthService(IAuthRepository userRepo, IJWTService jwtService, ILogger<AuthService> logger)
        {
            _authRepo = userRepo;
            _jwtService = jwtService;
            _logger = logger;
        }

        /// <summary>
        /// Creates a new user (admin provisioning).
        /// </summary>
        public async Task<int?> SignUpAsync(CreateUserDto signup)
        {
            try
            {
                _logger.LogInformation("Starting user sign-up for username: {Username}", signup.UserName);

                var hasher = new PasswordHasher<UserDto>();
                signup.PasswordHash = hasher.HashPassword(new UserDto(), signup.PasswordHash);

                var userId = await _authRepo.CreateUserAsync(signup);

                if (userId == null)
                {
                    _logger.LogWarning("User creation failed for username: {Username}", signup.UserName);
                }
                else
                {
                    _logger.LogInformation("User created successfully with ID: {UserId}", userId);
                }

                return userId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user sign-up for {Username}", signup.UserName);
                throw;
            }
        }

        /// <summary>
        /// Logs in a user and issues JWT if allowed.
        /// </summary>
        public async Task<LoginResultDto> LoginAsync(LoginDto loginCredentials)
        {
            try
            {
                _logger.LogInformation("Login attempt started for user/email: {UserInput}", loginCredentials.UsernameOrEmail);

                var user = await _authRepo.AuthenticateUserAsync(loginCredentials.UsernameOrEmail);
                if (user == null)
                {
                    return new LoginResultDto(false, null, "Invalid username or password", null);
                }

                // Check if active
                if (!user.IsActive)
                {
                    return new LoginResultDto(false, null, "Access revoked. Contact administrator.", null);
                }

                // Validate password
                var hasher = new PasswordHasher<UserDto>();
                var result = hasher.VerifyHashedPassword(user, user.PasswordHash, loginCredentials.Password);
                if (result != PasswordVerificationResult.Success)
                {
                    return new LoginResultDto(false, null, "Invalid username or password", null);
                }

                // Check if password update required
                if (user.MustChangePassword)
                {
                    if (user.TempPasswordExpiry.HasValue && user.TempPasswordExpiry < DateTime.UtcNow)
                    {
                        return new LoginResultDto(false, null, "Temporary password expired. Contact administrator.", null);
                    }

                    return new LoginResultDto(false, null, "Password change required before accessing application.", user);
                }

                // Generate JWT token
                var token = _jwtService.GenerateJwtToken(user);

                // Update last login
                await _authRepo.UpdateLastLoginAsync(user.UserId);

                return new LoginResultDto(true, token, null, user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception during login for {UserInput}", loginCredentials.UsernameOrEmail);
                throw;
            }
        }

        /// <summary>
        /// Allows user to change their password.
        /// </summary>
        public async Task<bool> ChangePasswordAsync(int userId, string oldPassword, string newPassword)
        {
            var user = await _authRepo.AuthenticateUserAsync(userId.ToString()); // Overload could be added for GetById
            if (user == null) return false;

            var hasher = new PasswordHasher<UserDto>();
            var result = hasher.VerifyHashedPassword(user, user.PasswordHash, oldPassword);
            if (result != PasswordVerificationResult.Success) return false;

            var hashedNew = hasher.HashPassword(user, newPassword);
            await _authRepo.ChangePasswordAsync(userId, hashedNew);

            return true;
        }

        /// <summary>
        /// Provision new temporary credentials (admin action).
        /// </summary>
        public async Task<bool> ProvisionAccessAsync(int userId, string tempPassword, TimeSpan expiry)
        {
            var hasher = new PasswordHasher<UserDto>();
            var hashed = hasher.HashPassword(new UserDto(), tempPassword);

            await _authRepo.UpdateTempCredentialsAsync(userId, hashed, DateTime.UtcNow.Add(expiry));

            _logger.LogInformation("Provisioned temp credentials for userId {UserId}", userId);

            return true;
        }

        /// <summary>
        /// Revoke user access (admin action).
        /// </summary>
        public async Task RevokeAccessAsync(int userId)
        {
            await _authRepo.RevokeAccessAsync(userId);
            _logger.LogInformation("Revoked access for userId {UserId}", userId);
        }
    }
}
