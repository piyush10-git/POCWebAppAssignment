using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCWebAppAssignment.Model.AuthDTOs
{
    public class UserWithRolesDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public List<string> Roles { get; set; } = new();
    }
}
