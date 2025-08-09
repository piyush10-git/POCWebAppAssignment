using Microsoft.AspNetCore.Identity;
using POCWebAppAssignment.Interfaces.Authentication;
using POCWebAppAssignment.Model.AuthDTOs;
using POCWebAppAssignment.Orchestration.HelperClasses;

namespace POCWebAppAssignment.Orchestration.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepo;
        private readonly IJWTService _jwtService;
        public AuthService(IAuthRepository authRepo, IJWTService jwtService)
        {
            _authRepo = authRepo;
            _jwtService = jwtService;
        }

        public async Task<int?> SignUpAsync(SignupDto signup)
        {
            signup.Password = _jwtService.HashPassword(signup.Password);
            return await _authRepo.CreateUserAaync(signup);
        }

        public async Task<LoginResultDto> LoginAsync(LoginDto loginCredentials)
        {
            // Get user details
            var user = await _authRepo.GetUserWithRolesAsync(loginCredentials.usernameOrEmail);

            if (user == null)
                return new LoginResultDto(false, null, "Invalid username or password", null);

            var hasher = new PasswordHasher<ApplicationUser>();
            var result = hasher.VerifyHashedPassword(
                new ApplicationUser(),
                user.PasswordHash,
                loginCredentials.Password
            );

            if(result != PasswordVerificationResult.Success)
            {
                return new LoginResultDto(false, null, "Invalid username or password", null);
            }

            var token = _jwtService.GenerateJwtToken(user);

            return new LoginResultDto(true, token, null, user);
        }
    }
}
