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

        public async Task<int?> CreateUserAsync(CreateUserDto dto)
        {
            var parameters = new List<SqlParameter>
            {
                SqlHelper.CreateSqlParameter("@UserName", dto.UserName),
                SqlHelper.CreateSqlParameter("@EmailId", dto.EmailId),
                SqlHelper.CreateSqlParameter("@PasswordHash", dto.PasswordHash),
                SqlHelper.CreateSqlParameter("@RoleId", dto.RoleId),
                SqlHelper.CreateSqlParameter("@EmpId", dto.EmpId),
                SqlHelper.CreateSqlParameter("@MustChangePassword", dto.MustChangePassword),
                SqlHelper.CreateSqlParameter("@TempPasswordExpiry", dto.TempPasswordExpiry),
                new SqlParameter("@UserId", SqlDbType.Int) { Direction = ParameterDirection.Output }
            };

            await SqlHelper.ExecuteStoredProcedureWithOutputAsync(_connectionString, "sp_CreateUser", parameters);

            var outputParam = parameters.First(p => p.ParameterName == "@UserId");
            return outputParam.Value != DBNull.Value ? (int)outputParam.Value : null;
        }

        public async Task<UserDto?> AuthenticateUserAsync(string usernameOrEmail)
        {
            var parameters = new List<SqlParameter>
            {
                SqlHelper.CreateSqlParameter("@EmailIdOrUsername", usernameOrEmail)
            };

            var dt = await SqlHelper.ExecuteDataTableAsync(_connectionString, "sp_AuthenticateUser", parameters);

            if (dt.Rows.Count == 0) return null;

            var row = dt.Rows[0];

            return new UserDto
            {
                UserId = row.Field<int>("UserId"),
                UserName = row.Field<string>("UserName"),
                PasswordHash = row.Field<string>("PasswordHash"),
                IsActive = row.Field<bool>("IsActive"),
                MustChangePassword = row.Field<bool>("MustChangePassword"),
                TempPasswordExpiry = row.Field<DateTime?>("TempPasswordExpiry"),
                LastLogin = row.Field<DateTime?>("LastLogin"),
                RoleName = row.Field<string>("RoleName")
            };
        }

        public async Task UpdateTempCredentialsAsync(int userId, string passwordHash, DateTime expiry)
        {
            var parameters = new List<SqlParameter>
            {
                SqlHelper.CreateSqlParameter("@UserId", userId),
                SqlHelper.CreateSqlParameter("@PasswordHash", passwordHash),
                SqlHelper.CreateSqlParameter("@TempPasswordExpiry", expiry)
            };

            await SqlHelper.ExecuteNonQueryAsync(_connectionString, "sp_UpdateTempCredentials", parameters);
        }

        public async Task ChangePasswordAsync(int userId, string passwordHash)
        {
            var parameters = new List<SqlParameter>
            {
                SqlHelper.CreateSqlParameter("@UserId", userId),
                SqlHelper.CreateSqlParameter("@PasswordHash", passwordHash)
            };

            await SqlHelper.ExecuteNonQueryAsync(_connectionString, "sp_ChangePassword", parameters);
        }

        public async Task RevokeAccessAsync(int userId)
        {
            var parameters = new List<SqlParameter>
            {
                SqlHelper.CreateSqlParameter("@UserId", userId)
            };

            await SqlHelper.ExecuteNonQueryAsync(_connectionString, "sp_RevokeAccess", parameters);
        }

        public async Task UpdateLastLoginAsync(int userId)
        {
            var parameters = new List<SqlParameter>
            {
                SqlHelper.CreateSqlParameter("@UserId", userId)
            };

            await SqlHelper.ExecuteNonQueryAsync(_connectionString, "sp_UpdateLastLogin", parameters);
        }
    }
}
