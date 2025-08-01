using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using POCWebAppAssignment.Model;
using POCWebAppAssignment.Model.DTOs;
using POCWebAppAssignment.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text.Json;
using System.Threading.Tasks;
using Serilog;

namespace POCWebAppAssignment.Repository.RunStoredProcedures
{
    public class RunStoredProceduresNormalizedTable : IRunStoredProceduresNormalizedTable
    {
        private readonly string _connectionString;
         public RunStoredProceduresNormalizedTable(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DB_Connection");
        }

        public async Task<IEnumerable<Resource>> GetAll()
        {
            List<Resource> response = new List<Resource>();
            DataTable employeeTable = new DataTable();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("GetAllResources", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        employeeTable.Load(reader);
                    }
                }
            }
            foreach (DataRow row in employeeTable.Rows)
            {
                Resource details = new Resource();
                details.EmpId = (int)row["EmpId"];
                details.ResourceName = row["ResourceName"].ToString();
                details.Designation = row["Designation"].ToString();
                details.ReportingTo = row["ReportingTo"].ToString();
                details.TechnologySkill = row["TechnologySkill"].ToString();
                details.ProjectAllocation = row["ProjectAllocation"].ToString();
                details.Location = row["Location"].ToString();
                details.EmailId = row["EmailId"].ToString();
                details.Remarks = row["Remarks"].ToString();
                details.Billable = Convert.ToBoolean(row["Billable"]);
                details.CteDoj = DateOnly.FromDateTime(Convert.ToDateTime(row["CteDoj"]));
                response.Add(details);
            }
            return response;
        }

        public async Task<int> CreateResource(ResourceDto details)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("CreateResource", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            // Standard scalar parameters
            command.Parameters.AddWithValue("@ResourceName", details.ResourceName);
            command.Parameters.AddWithValue("@DesignationId", details.Designation);
            command.Parameters.AddWithValue("@ReportingTo", details.ReportingTo ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@LocationId", details.Location);
            command.Parameters.AddWithValue("@EmailId", details.EmailId);
            command.Parameters.AddWithValue("@Remarks", details.Remarks ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Billable", details.Billable);
            command.Parameters.AddWithValue("@CteDoj", details.CteDoj);

            // OUTPUT parameter
            var outputIdParam = new SqlParameter("@EmpId", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            command.Parameters.Add(outputIdParam);

            // SkillIds as TVP
            var skillIdsTable = ToIdDataTable(details.TechnologySkill);
            var skillParam = command.Parameters.AddWithValue("@SkillIds", skillIdsTable);
            skillParam.SqlDbType = SqlDbType.Structured;
            skillParam.TypeName = "dbo.IdList";

            // ProjectIds as TVP
            var projectIdsTable = ToIdDataTable(details.ProjectAllocation);
            var projectParam = command.Parameters.AddWithValue("@ProjectIds", projectIdsTable);
            projectParam.SqlDbType = SqlDbType.Structured;
            projectParam.TypeName = "dbo.IdList";

            // Execute
            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();

            return (int)outputIdParam.Value;
        }

        private static DataTable ToIdDataTable(IEnumerable<int> ids)
        {
            var table = new DataTable();
            table.Columns.Add("Id", typeof(int));
            foreach (var id in ids ?? Enumerable.Empty<int>())
            {
                table.Rows.Add(id);
            }
            return table;
        }




        public async Task<ResourceDetailsDto> GetResourceById(int empId)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("GetResourceById", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@EmpId", empId);

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                // Get the full JSON result from SQL
                string json = reader.GetString(0);

                // Deserialize into your DTO
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                return JsonSerializer.Deserialize<ResourceDetailsDto>(json, options);
            }

            return null;
        }


        public async Task UpdateResource(ResourceDto details)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("UpdateResource", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            // Scalar parameters
            command.Parameters.AddWithValue("@EmpId", details.EmpId);
            command.Parameters.AddWithValue("@ResourceName", details.ResourceName);
            command.Parameters.AddWithValue("@DesignationId", details.Designation);
            command.Parameters.AddWithValue("@ReportingTo", details.ReportingTo ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@LocationId", details.Location);
            command.Parameters.AddWithValue("@EmailId", details.EmailId);
            command.Parameters.AddWithValue("@Remarks", details.Remarks ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Billable", details.Billable);
            command.Parameters.AddWithValue("@CteDoj", details.CteDoj);

            // Skills TVP
            var skillIds = ToIdDataTable(details.TechnologySkill);
            var skillParam = command.Parameters.AddWithValue("@SkillIds", skillIds);
            skillParam.SqlDbType = SqlDbType.Structured;
            skillParam.TypeName = "dbo.IdList";

            // Projects TVP
            var projectIds = ToIdDataTable(details.ProjectAllocation);
            var projectParam = command.Parameters.AddWithValue("@ProjectIds", projectIds);
            projectParam.SqlDbType = SqlDbType.Structured;
            projectParam.TypeName = "dbo.IdList";

            // Execute
            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }


        public async Task DeleteResource(int empId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("DeleteResourceById", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@EmpId", empId);

                    connection.Open();
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task DeleteResourcesByEmpIdList(IEnumerable<int> empIds)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("DeleteResourcesByEmpIdList", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Create DataTable to match EmpIdList type
                    var empIdTable = new DataTable();
                    empIdTable.Columns.Add("EmpId", typeof(int));

                    foreach (var id in empIds)
                    {
                        empIdTable.Rows.Add(id);
                    }

                    var param = new SqlParameter("@EmpIds", SqlDbType.Structured)
                    {
                        TypeName = "EmpIdList",
                        Value = empIdTable
                    };

                    command.Parameters.Add(param);

                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<bool> CheckEmailExists(string emailId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("CheckIfEmailExists", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@EmailId", emailId);


                    SqlParameter doesExist = new SqlParameter("@DoesExist", SqlDbType.Bit)
                    {
                        Direction = ParameterDirection.Output
                    };


                    command.Parameters.Add(doesExist);
                    connection.Open();
                    await command.ExecuteNonQueryAsync();
                    return (bool)doesExist.Value;
                }
            }
        }

        public async Task<IEnumerable<int>> GetResourceStatistics()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("GetResourceStatistics", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Add OUTPUT parameter for EmpId
                    SqlParameter outputTotalResourceParam = new SqlParameter("@Total_Resources", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    SqlParameter outputTotalBillableParam = new SqlParameter("@Total_Billable", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    SqlParameter outputTotalProjectsParam = new SqlParameter("@Total_Projects", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(outputTotalResourceParam);
                    command.Parameters.Add(outputTotalBillableParam);
                    command.Parameters.Add(outputTotalProjectsParam);

                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();

                    List<int> result = new List<int>();
                    result.Add((int)outputTotalResourceParam.Value);
                    result.Add((int)outputTotalBillableParam.Value);
                    result.Add((int)outputTotalProjectsParam.Value);

                    // Retrieve the output value
                    return result;
                }
            }
        }

        public async Task<DropdownResponseDto> GetDropdownData()
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("GetDropdownDataJson", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            await conn.OpenAsync();

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                string json = reader.GetString(0);

                Console.WriteLine(json);

                return JsonSerializer.Deserialize<DropdownResponseDto>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }

            return new DropdownResponseDto(); // return empty object on no data
        }

        public async Task<int> BulkUpdateResources(BulkEditDto bulkEditDetails)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("BulkUpdateResources", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            // Required: ResourceIds TVP
            var resourceIdParam = command.Parameters.AddWithValue("@ResourceIds", ToIdDataTable(bulkEditDetails.ResourceIds));
            resourceIdParam.SqlDbType = SqlDbType.Structured;
            resourceIdParam.TypeName = "dbo.IdList";

            // Optional scalar values
            command.Parameters.AddWithValue("@DesignationId", (object?)bulkEditDetails.FeildsToEdit.DesignationId ?? DBNull.Value);
            command.Parameters.AddWithValue("@LocationId", (object?)bulkEditDetails.FeildsToEdit.LocationId ?? DBNull.Value);
            command.Parameters.AddWithValue("@ReportingTo", (object?)bulkEditDetails.FeildsToEdit.ReportingTo ?? DBNull.Value);
            command.Parameters.AddWithValue("@Billable", (object?)bulkEditDetails.FeildsToEdit.Billable ?? DBNull.Value);
            command.Parameters.AddWithValue("@CteDoj", bulkEditDetails.FeildsToEdit.CteDoj.HasValue
                ? (object)bulkEditDetails.FeildsToEdit.CteDoj.Value.ToDateTime(TimeOnly.MinValue)
                : DBNull.Value);
            command.Parameters.AddWithValue("@Remarks", (object?)bulkEditDetails.FeildsToEdit.Remarks ?? DBNull.Value);

            // Always pass TVPs, even if empty
            var skillIds = ToIdDataTable(bulkEditDetails.FeildsToEdit.SkillIds ?? new List<int>());
            var skillParam = command.Parameters.AddWithValue("@SkillIds", skillIds);
            skillParam.SqlDbType = SqlDbType.Structured;
            skillParam.TypeName = "dbo.IdList";

            var projectIds = ToIdDataTable(bulkEditDetails.FeildsToEdit.ProjectIds ?? new List<int>());
            var projectParam = command.Parameters.AddWithValue("@ProjectIds", projectIds);
            projectParam.SqlDbType = SqlDbType.Structured;
            projectParam.TypeName = "dbo.IdList";

            await connection.OpenAsync();

            var result = await command.ExecuteReaderAsync();
            int updatedCount = 0;

            if (await result.ReadAsync())
            {
                updatedCount = result.GetInt32(0);
            }

            return updatedCount;
        }

        //private static DataTable ToIdDataTable(IEnumerable<int> ids)
        //{
        //    var table = new DataTable();
        //    table.Columns.Add("Id", typeof(int));
        //    foreach (var id in ids ?? Enumerable.Empty<int>())
        //        table.Rows.Add(id);
        //    return table;
        //}


        public async Task BulkCreateResourcesAsync(List<ResourceDto> resources)
        {
            try
            {
                var employeesTable = new DataTable();
                employeesTable.Columns.Add("TempKey", typeof(Guid));
                employeesTable.Columns.Add("ResourceName", typeof(string));
                employeesTable.Columns.Add("DesignationId", typeof(int));
                employeesTable.Columns.Add("ReportingTo", typeof(string));
                employeesTable.Columns.Add("Billable", typeof(bool));
                employeesTable.Columns.Add("LocationId", typeof(int));
                employeesTable.Columns.Add("EmailId", typeof(string));
                employeesTable.Columns.Add("CteDoj", typeof(DateTime));
                employeesTable.Columns.Add("Remarks", typeof(string));

                var skillsTable = new DataTable();
                skillsTable.Columns.Add("TempKey", typeof(Guid));
                skillsTable.Columns.Add("SkillId", typeof(int));

                var projectsTable = new DataTable();
                projectsTable.Columns.Add("TempKey", typeof(Guid));
                projectsTable.Columns.Add("ProjectId", typeof(int));

                foreach (var resource in resources)
                {
                    var tempKey = Guid.NewGuid();

                    employeesTable.Rows.Add(
                        tempKey,
                        resource.ResourceName,
                        resource.Designation,
                        resource.ReportingTo ?? (object)DBNull.Value,
                        resource.Billable,
                        resource.Location,
                        resource.EmailId,
                        resource.CteDoj.ToDateTime(new TimeOnly(0, 0)),
                        resource.Remarks ?? string.Empty
                    );

                    if (resource.TechnologySkill != null)
                    {
                        foreach (var skillId in resource.TechnologySkill)
                        {
                            skillsTable.Rows.Add(tempKey, skillId);
                        }
                    }

                    if (resource.ProjectAllocation != null)
                    {
                        foreach (var projectId in resource.ProjectAllocation)
                        {
                            projectsTable.Rows.Add(tempKey, projectId);
                        }
                    }
                }

                using var connection = new SqlConnection(_connectionString);
                using var command = new SqlCommand("sp_BulkImport", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                var employeesParam = command.Parameters.AddWithValue("@Employees", employeesTable);
                employeesParam.SqlDbType = SqlDbType.Structured;
                employeesParam.TypeName = "dbo.TempEmployee";

                var skillsParam = command.Parameters.AddWithValue("@Skills", skillsTable);
                skillsParam.SqlDbType = SqlDbType.Structured;
                skillsParam.TypeName = "dbo.TempEmployeeSkills";

                var projectsParam = command.Parameters.AddWithValue("@Projects", projectsTable);
                projectsParam.SqlDbType = SqlDbType.Structured;
                projectsParam.TypeName = "dbo.TempProjectAllocation";

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public async Task<List<OptionDto>?> GetRoleOptionsDropDownAsync()
        {
            using var conn = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_GetRoleOptions", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            var result = new SqlParameter("@result", SqlDbType.NVarChar, -1)
            {
                Direction = ParameterDirection.Output
            };
            command.Parameters.Add(result);

            await conn.OpenAsync();
            await command.ExecuteNonQueryAsync();

            //string json = Convert.ToString(result.Value);
            string json = (string)result.Value;

            if (!string.IsNullOrWhiteSpace(json))
            {
                var value = JsonSerializer.Deserialize<List<OptionDto>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });;
                return value;
            }

            return new List<OptionDto>();
        }


    }
}
