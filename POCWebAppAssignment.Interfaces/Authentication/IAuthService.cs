using POCWebAppAssignment.Model.AuthDTOs;

namespace POCWebAppAssignment.Interfaces.Authentication
{
    public interface IAuthService
    {
        Task<bool> ChangePasswordAsync(int userId, string oldPassword, string newPassword);
        Task<LoginResultDto> LoginAsync(LoginDto loginCredentials);
        Task<bool> ProvisionAccessAsync(int userId, string tempPassword, TimeSpan expiry);
        Task RevokeAccessAsync(int userId);
        Task<int?> SignUpAsync(CreateUserDto signup);
    }
}