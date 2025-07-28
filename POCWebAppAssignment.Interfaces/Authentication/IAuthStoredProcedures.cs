using POCWebAppAssignment.Model.AuthDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCWebAppAssignment.Interfaces.Authentication
{
    public interface IAuthStoredProcedures
    {
        Task<UserWithRolesDto?> GetUserWithRolesAsync(string UserName, string Password);
        Task<int?> CreateUserAsync(SignupDto signup);
    }
}
