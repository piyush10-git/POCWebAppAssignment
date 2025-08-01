﻿using POCWebAppAssignment.Model.AuthDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCWebAppAssignment.Interfaces.Authentication
{
    public interface IAuthRepository
    {
        Task<UserWithRolesDto?> GetUserWithRolesAsync(string UserName, string Password);
        Task<int?> CreateUserAaync(SignupDto signup);
    }
}
