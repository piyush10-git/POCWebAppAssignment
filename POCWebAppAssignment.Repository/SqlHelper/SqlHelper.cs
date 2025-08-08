using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace POCWebAppAssignment.Helpers
{
    public class SqlHelper
    {
        private readonly string _connectionString;

        public SqlHelper(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Execute NonQuery (Insert, Update, Delete)
        public async Task<int> ExecuteNonQueryAsync(string storedProcedure, Dictionary<string, object> parameters = null, Dictionary<string, SqlParameter> outputParameters = null)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(storedProcedure, connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            AddParameters(command, parameters);
            AddOutputParameters(command, outputParameters);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();

            // Return output parameter value if needed
            if (outputParameters != null)
            {
                foreach (var param in outputParameters)
                {
                    param.Value.Value = command.Parameters[param.Key].Value;
                }
            }

            return 1;
        }

        // Execute Reader and return DataTable
        public async Task<DataTable> ExecuteReaderAsync(string storedProcedure, Dictionary<string, object> parameters = null)
        {
            var dataTable = new DataTable();

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(storedProcedure, connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            AddParameters(command, parameters);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            dataTable.Load(reader);

            return dataTable;
        }

        // Execute Reader and return SqlDataReader (for custom row reading)
        public async Task<SqlDataReader> ExecuteReaderRawAsync(string storedProcedure, Dictionary<string, object> parameters = null)
        {
            var connection = new SqlConnection(_connectionString);
            var command = new SqlCommand(storedProcedure, connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            AddParameters(command, parameters);

            await connection.OpenAsync();
            return await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
        }

        // Helper to add input parameters
        private void AddParameters(SqlCommand command, Dictionary<string, object> parameters)
        {
            if (parameters == null) return;

            foreach (var param in parameters)
            {
                var value = param.Value ?? DBNull.Value;
                command.Parameters.AddWithValue(param.Key, value);
            }
        }

        // Helper to add output parameters
        private void AddOutputParameters(SqlCommand command, Dictionary<string, SqlParameter> outputParameters)
        {
            if (outputParameters == null) return;

            foreach (var param in outputParameters)
            {
                param.Value.Direction = ParameterDirection.Output;
                command.Parameters.Add(param.Value);
            }
        }

        // Execute with table-valued parameter
        public async Task ExecuteWithDataTablesAsync(string storedProcedure, Dictionary<string, DataTable> tvps)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(storedProcedure, connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            foreach (var tvp in tvps)
            {
                var param = command.Parameters.AddWithValue(tvp.Key, tvp.Value);
                param.SqlDbType = SqlDbType.Structured;
            }

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }
    }
}
