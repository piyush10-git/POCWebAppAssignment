using POCWebAppAssignment.Model.AuthDTOs;

namespace POCWebAppAssignment.Interfaces.Authentication
{
    public interface IJWTService
    {
        string GenerateJwtToken(UserWithRolesDto user);
        string HashPassword(string password);
    }
}