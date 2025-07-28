using POCWebAppAssignment.Model.AuthDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCWebAppAssignment.Interfaces.Authentication
{
    public interface IAuthService
    {
        Task<LoginResultDto> LoginAsync(LoginDto loginCredentials);
        Task<int?> SignUpAsync(SignupDto signup);
    }
}
