using POCWebAppAssignment.Model.AuthDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCWebAppAssignment.Interfaces.Authentication
{
    public interface IAuthRepository
    {
        Task<UserWithRolesDto?> GetUserWithRolesAsync(string UserName);
        Task<int?> CreateUserAsync(SignupDto signup);
    }
}
