using Microsoft.Extensions.Logging;
using POCWebAppAssignment.Interfaces.Authentication;
using POCWebAppAssignment.Model.AuthDTOs;

namespace POCWebAppAssignment.Repository.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly IAuthStoredProcedures _authStoredProcedure;
        private readonly ILogger<AuthRepository> _logger;

        public AuthRepository(IAuthStoredProcedures authStoredProcedure, ILogger<AuthRepository> logger)
        {
            _authStoredProcedure = authStoredProcedure;
            _logger = logger;
        }

        public async Task<UserWithRolesDto?> GetUserWithRolesAsync(string username)
        {
            try
            {
                _logger.LogInformation("Fetching user with roles for username/email: {Username}", username);

                var user = await _authStoredProcedure.GetUserWithRolesAsync(username);

                if (user == null)
                {
                    _logger.LogWarning("No user found for: {Username}", username);
                }
                else
                {
                    _logger.LogInformation("User with roles fetched successfully for: {Username}", username);
                }

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching user with roles for: {Username}", username);
                throw;
            }
        }

        public async Task<int?> CreateUserAsync(SignupDto signup)
        {
            try
            {
                _logger.LogInformation("Attempting to create user with username: {Username}", signup.UserName);

                var userId = await _authStoredProcedure.CreateUserAsync(signup);

                if (userId == null)
                {
                    _logger.LogWarning("User creation returned null for username: {Username}", signup.UserName);
                }
                else
                {
                    _logger.LogInformation("User created successfully with ID: {UserId} for username: {Username}", userId, signup.UserName);
                }

                return userId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating user: {Username}", signup.UserName);
                throw;
            }
        }
    }
}
