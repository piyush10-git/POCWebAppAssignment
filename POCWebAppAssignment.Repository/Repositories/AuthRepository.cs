using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using POCWebAppAssignment.Interfaces.Authentication;
using POCWebAppAssignment.Model.AuthDTOs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCWebAppAssignment.Repository.Repositories
{
    public class AuthRepository: IAuthRepository
    {
        private readonly IAuthStoredProcedures _authStoredProcedure;
        public AuthRepository(IAuthStoredProcedures authStoredProcedure)
        {
            _authStoredProcedure = authStoredProcedure;
        }

        public async Task<UserWithRolesDto?> GetUserWithRolesAsync(string UserName, string Password)
        {
            return await _authStoredProcedure.GetUserWithRolesAsync(UserName, Password);
        }

        public async Task<int?> CreateUserAaync(SignupDto signup)
        {
            return await _authStoredProcedure.CreateUserAsync(signup);
        }
    }
}
