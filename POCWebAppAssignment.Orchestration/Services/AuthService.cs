using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using POCWebAppAssignment.Interfaces;
using POCWebAppAssignment.Interfaces.Authentication;
using POCWebAppAssignment.Model.AuthDTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Text;


namespace POCWebAppAssignment.Orchestration.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepo;
        private readonly IConfiguration _config;
        private readonly JwtConfig _JWTConfig;

        public AuthService(IAuthRepository authRepo, IConfiguration config)
        {
            _authRepo = authRepo;
            _config = config;

            _JWTConfig = new JwtConfig
            {
                jwtKey = _config["Jwt:Key"] ?? throw new InvalidOperationException("JWT key not configured."),
                issuer = _config["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT issuer not configured."),
                audience = _config["Jwt:Audience"] ?? throw new InvalidOperationException("JWT audience not configured."),
                expiresIn = _config["Jwt:ExpiresInMinutes"] ?? throw new InvalidOperationException("JWT expiration not configured."),
            };
        }

        public async Task<int?> SignUpAsync(SignupDto signup)
        {
            signup.Password = HashPassword(signup.Password);
            return await _authRepo.CreateUserAaync(signup);
        }

        public async Task<LoginResultDto> LoginAsync(LoginDto loginCredentials)
        {
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

            var token = GenerateJwtToken(user);

            return new LoginResultDto(true, token, null, user);
        }

        private string GenerateJwtToken(UserWithRolesDto user)
        {
            var claims = new List<Claim> {
                new Claim("UserName", user.UserName),
                new Claim("UserId", user.UserId.ToString())
            };

            claims.AddRange(user.Roles.Select(role => new Claim("role", role)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_JWTConfig.jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _JWTConfig.issuer,
                audience: _JWTConfig.audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_JWTConfig.expiresIn)),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string HashPassword(string password)
        {
            var hasher = new PasswordHasher<ApplicationUser>();
            return hasher.HashPassword(new ApplicationUser(), password);
        }
    }
}
