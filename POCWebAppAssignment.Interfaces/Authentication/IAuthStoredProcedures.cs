using POCWebAppAssignment.Model.AuthDTOs;

namespace POCWebAppAssignment.Interfaces.Authentication
{
    public interface IAuthStoredProcedures
    {
        Task<UserDto?> AuthenticateUserAsync(string usernameOrEmail);
        Task ChangePasswordAsync(int userId, string passwordHash);
        Task<int?> CreateUserAsync(CreateUserDto dto);
        Task RevokeAccessAsync(int userId);
        Task UpdateLastLoginAsync(int userId);
        Task UpdateTempCredentialsAsync(int userId, string passwordHash, DateTime expiry);
    }
}