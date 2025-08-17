using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCWebAppAssignment.Model.AuthDTOs
{
    public class UserDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string EmailId { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public bool MustChangePassword { get; set; }
        public DateTime? TempPasswordExpiry { get; set; }
        public DateTime? LastLogin { get; set; }
        public string RoleName { get; set; } = string.Empty;
    }

    public class CreateUserDto
    {
        public string UserName { get; set; } = string.Empty;
        public string EmailId { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public int RoleId { get; set; }
        public int? EmpId { get; set; }
        public bool MustChangePassword { get; set; } = true;
        public DateTime? TempPasswordExpiry { get; set; }
    }

    public class LoginDto
    {
        public string UsernameOrEmail { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class LoginResultDto
    {
        public bool Success { get; }
        public string? Token { get; }
        public string? Message { get; }
        public UserDto? User { get; }

        public LoginResultDto(bool success, string? token, string? message, UserDto? user)
        {
            Success = success;
            Token = token;
            Message = message;
            User = user;
        }
    }

    public class ChangePasswordDto
    {
        public int UserId { get; set; }
        public string OldPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}
