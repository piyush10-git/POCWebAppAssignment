using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using POCWebAppAssignment.Model.AuthDTOs;
using POCWebAppAssignment.Interfaces.Authentication;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace POCWebAppAssignment.Orchestration.HelperClasses
{
    public class JWTService : IJWTService
    {
        private readonly string _jwtKey;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly int _expiresInMinutes;

        public JWTService(IConfiguration config)
        {
            _jwtKey = config["Jwt:Key"] ?? throw new InvalidOperationException("JWT key not configured.");
            _issuer = config["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT issuer not configured.");
            _audience = config["Jwt:Audience"] ?? throw new InvalidOperationException("JWT audience not configured.");

            if (!int.TryParse(config["Jwt:ExpiresInMinutes"], out _expiresInMinutes))
            {
                throw new InvalidOperationException("Invalid JWT expiration configuration.");
            }
        }

        public string GenerateJwtToken(UserDto user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, user.RoleName)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_expiresInMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
