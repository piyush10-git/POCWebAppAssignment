using POCWebAppAssignment.Model.AuthDTOs;

namespace POCWebAppAssignment.Interfaces.Authentication
{
    public interface IAuthService
    {
        Task<LoginResultDto> LoginAsync(LoginDto loginCredentials);
        Task<int?> SignUpAsync(SignupDto signup);
    }
}
