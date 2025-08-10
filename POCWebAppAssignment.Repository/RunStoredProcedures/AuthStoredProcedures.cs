using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using POCWebAppAssignment.Repository.Helpers;
using POCWebAppAssignment.Interfaces.Authentication;
using POCWebAppAssignment.Model.AuthDTOs;
using System.Data;

namespace POCWebAppAssignment.Repository.RunStoredProcedures
{
    public class AuthStoredProcedures : IAuthStoredProcedures
    {
        private readonly string _connectionString;

        public AuthStoredProcedures(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DB_Connection");
        }

        public async Task<UserWithRolesDto?> GetUserWithRolesAsync(string userNameOrEmail)
        {
            var parameters = new List<SqlParameter>
            {
                SqlHelper.CreateSqlParameter("@EmailIdOrUsername", userNameOrEmail)
            };

            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_AuthenticateUser", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddRange(parameters.ToArray());

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            if (!await reader.ReadAsync()) return null;

            var user = new UserWithRolesDto
            {
                UserId = reader.GetInt32(0),
                UserName = reader.GetString(1),
                PasswordHash = reader.GetString(2)
            };

            if (await reader.NextResultAsync())
            {
                while (await reader.ReadAsync())
                {
                    user.Roles.Add(reader.GetString(0));
                }
            }

            return user;
        }

        public async Task<int?> CreateUserAsync(SignupDto signup)
        {
            var parameters = new List<SqlParameter>
            {
                SqlHelper.CreateSqlParameter("@UserName", signup.UserName),
                SqlHelper.CreateSqlParameter("@EmailId", signup.Email),
                SqlHelper.CreateSqlParameter("@PasswordHash", signup.Password),
                SqlHelper.CreateSqlParameter("@RoleId", signup.RoleId),
                new SqlParameter("@UserId", SqlDbType.Int) { Direction = ParameterDirection.Output }
            };

            await SqlHelper.ExecuteStoredProcedureWithOutputAsync(_connectionString, "sp_CreateUser", parameters);

            return parameters.First(p => p.ParameterName == "@UserId").Value is int id ? id : null;
        }
    }
}
