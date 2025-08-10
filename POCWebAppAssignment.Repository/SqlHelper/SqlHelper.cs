using Microsoft.Data.SqlClient;
using System.Data;
using System.Text.Json;

namespace POCWebAppAssignment.Repository.Helpers
{
    public static class SqlHelper
    {
        // Executes a stored procedure and returns a DataTable
        public static async Task<DataTable> ExecuteDataTableAsync(string connectionString, string storedProcedure, IEnumerable<SqlParameter>? parameters = null)
        {
            var dataTable = new DataTable();

            using var connection = new SqlConnection(connectionString);
            using var command = new SqlCommand(storedProcedure, connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            if (parameters != null)
                command.Parameters.AddRange(parameters.ToArray());

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            dataTable.Load(reader);

            return dataTable;
        }

        // Executes a stored procedure with output parameters
        public static async Task ExecuteStoredProcedureWithOutputAsync(string connectionString, string storedProcedure, List<SqlParameter> parameters)
        {
            using var connection = new SqlConnection(connectionString);
            using var command = new SqlCommand(storedProcedure, connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddRange(parameters.ToArray());

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        // Executes a stored procedure that returns a single JSON string, and deserializes it
        public static async Task<T?> DeserializeJsonFromReaderAsync<T>(string connectionString, string storedProcedure, IEnumerable<SqlParameter>? parameters = null)
        {
            using var connection = new SqlConnection(connectionString);
            using var command = new SqlCommand(storedProcedure, connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            if (parameters != null)
                command.Parameters.AddRange(parameters.ToArray());

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                var json = reader.GetString(0);

                return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }

            return default;
        }

        // Execute a stored procedure returning a scalar value
        public static async Task<T> ExecuteScalarAsync<T>(string connectionString, string storedProcedure, IEnumerable<SqlParameter>? parameters = null)
        {
            using var connection = new SqlConnection(connectionString);
            using var command = new SqlCommand(storedProcedure, connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            if (parameters != null)
                command.Parameters.AddRange(parameters.ToArray());

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            return (T)Convert.ChangeType(result, typeof(T));
        }

        // Executes a stored procedure that performs a non-query (insert/update/delete)
        public static async Task ExecuteNonQueryAsync(string connectionString, string storedProcedure, IEnumerable<SqlParameter>? parameters = null)
        {
            using var connection = new SqlConnection(connectionString);
            using var command = new SqlCommand(storedProcedure, connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            if (parameters != null)
                command.Parameters.AddRange(parameters.ToArray());

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        // Helper: Converts a list of ints to a DataTable for TVP use
        public static DataTable CreateIdDataTable(IEnumerable<int> ids, string columnName = "Id")
        {
            var table = new DataTable();
            table.Columns.Add(columnName, typeof(int));

            foreach (var id in ids ?? Enumerable.Empty<int>())
            {
                table.Rows.Add(id);
            }

            return table;
        }

        // Helper: Safely create SQL parameter with DBNull handling
        public static SqlParameter CreateSqlParameter(string name, object? value)
        {
            return new SqlParameter(name, value ?? DBNull.Value);
        }

        // Helper: Create TVP SqlParameter
        public static SqlParameter CreateTvpParameter(string name, DataTable table, string typeName)
        {
            return new SqlParameter(name, SqlDbType.Structured)
            {
                TypeName = typeName,
                Value = table
            };
        }
    }
}
