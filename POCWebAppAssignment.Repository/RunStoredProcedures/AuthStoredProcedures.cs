using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using POCWebAppAssignment.Interfaces.Authentication;
using POCWebAppAssignment.Model;
using POCWebAppAssignment.Model.AuthDTOs;
using POCWebAppAssignment.Model.DTOs;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography.Pkcs;
using System.Text.Json;
using System.Threading.Tasks;

namespace POCWebAppAssignment.Repository.RunStoredProcedures
{
    public class AuthStoredProcedures: IAuthStoredProcedures
    {
        private readonly string _connectionString;
        public AuthStoredProcedures(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DB_Connection");
        }

        public async Task<UserWithRolesDto?> GetUserWithRolesAsync(string UserNameOrEmail, string Password)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new SqlCommand("sp_AuthenticateUser", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@EmailIdOrUsername", UserNameOrEmail);
            cmd.Parameters.AddWithValue("@PasswordHash", Password);

            using var reader = await cmd.ExecuteReaderAsync();

            if (!await reader.ReadAsync()) return null;

            var user = new UserWithRolesDto
            {
                UserId = reader.GetInt32(0),
                UserName = reader.GetString(1)
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
            using var conn = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_CreateUser", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@UserName", signup.UserName);
            command.Parameters.AddWithValue("@EmailId", signup.Email);
            command.Parameters.AddWithValue("@PasswordHash", signup.Password); // Use a hashed value
            command.Parameters.AddWithValue("@RoleId", signup.RoleId); // FIXED: add missing parameter

            var outputParam = new SqlParameter("@UserId", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };

            command.Parameters.Add(outputParam);

            await conn.OpenAsync();
            await command.ExecuteNonQueryAsync();

            return outputParam.Value != DBNull.Value ? (int?)outputParam.Value : null;
        }


    }
}
