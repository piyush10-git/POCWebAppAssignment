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

        public async Task<UserDto?> AuthenticateUserAsync(string usernameOrEmail)
        {
            return await _authStoredProcedure.AuthenticateUserAsync(usernameOrEmail);
        }

        public async Task ChangePasswordAsync(int userId, string passwordHash)
        {
            await _authStoredProcedure.ChangePasswordAsync(userId, passwordHash);
        }

        public async Task<int?> CreateUserAsync(CreateUserDto dto)
        {
            return await _authStoredProcedure.CreateUserAsync(dto);
        }

        public async Task RevokeAccessAsync(int userId)
        {
            await _authStoredProcedure.RevokeAccessAsync(userId);
        }

        public async Task UpdateLastLoginAsync(int userId)
        {
            await _authStoredProcedure.UpdateLastLoginAsync(userId);
        }

        public async Task UpdateTempCredentialsAsync(int userId, string passwordHash, DateTime expiry)
        {
            await _authStoredProcedure.UpdateTempCredentialsAsync(userId, passwordHash, expiry);
        }
    }
}
