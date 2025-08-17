using POCWebAppAssignment.Model.AuthDTOs;

namespace POCWebAppAssignment.Interfaces.Authentication
{
    public interface IJWTService
    {
        string GenerateJwtToken(UserDto user);
    }
}