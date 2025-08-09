using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using POCWebAppAssignment.Model.AuthDTOs;
using POCWebAppAssignment.Interfaces.Authentication;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace POCWebAppAssignment.Orchestration.HelperClasses
{
    public class JWTService : IJWTService
    {
        string jwtKey, issuer, audience, expiresIn;
        public JWTService(IConfiguration config)
        {
            jwtKey = config["Jwt:Key"] ?? throw new InvalidOperationException("JWT key not configured.");
            issuer = config["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT issuer not configured.");
            audience = config["Jwt:Audience"] ?? throw new InvalidOperationException("JWT audience not configured.");
            expiresIn = config["Jwt:ExpiresInMinutes"] ?? throw new InvalidOperationException("JWT expiration not configured.");
        }

        public string GenerateJwtToken(UserWithRolesDto user)
        {
            var claims = new List<Claim> {
                new Claim("UserName", user.UserName),
                new Claim("UserId", user.UserId.ToString())
            };

            claims.AddRange(user.Roles.Select(role => new Claim("role", role)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(expiresIn)),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string HashPassword(string password)
        {
            var hasher = new PasswordHasher<ApplicationUser>();
            return hasher.HashPassword(new ApplicationUser(), password);
        }

    }
}
