using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using POCWebAppAssignment.Interfaces;
using POCWebAppAssignment.Model;
using POCWebAppAssignment.Model.DTOs;
using POCWebAppAssignment.Repository.Helpers;
using System.Data;
using System.Text.Json;

namespace POCWebAppAssignment.Repository.RunStoredProcedures
{
    public class ResourceProcedureService : IResourceProcedureService
    {
        private readonly string _connectionString;

        public ResourceProcedureService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DB_Connection");
        }

        public async Task<IEnumerable<Resource>> GetAll()
        {
            var table = await SqlHelper.ExecuteDataTableAsync(_connectionString, "GetAllResources1");

            var response = new List<Resource>();
            foreach (DataRow row in table.Rows)
            {
                response.Add(new Resource
                {
                    EmpId = (int)row["EmpId"],
                    ResourceName = row["ResourceName"].ToString(),
                    Designation = row["Designation"].ToString(),
                    ReportingTo = row["ReportingTo"].ToString(),
                    TechnologySkill = row["TechnologySkill"].ToString(),
                    ProjectAllocation = row["ProjectAllocation"].ToString(),
                    Location = row["Location"].ToString(),
                    EmailId = row["EmailId"].ToString(),
                    Remarks = row["Remarks"].ToString(),
                    Billable = Convert.ToBoolean(row["Billable"]),
                    CteDoj = DateOnly.FromDateTime(Convert.ToDateTime(row["CteDoj"]))
                });
            }

            return response;
        }

        public async Task<int> CreateResource(ResourceDto details)
        {
            var parameters = new List<SqlParameter>
            {
                SqlHelper.CreateSqlParameter("@ResourceName", details.ResourceName),
                SqlHelper.CreateSqlParameter("@DesignationId", details.Designation),
                SqlHelper.CreateSqlParameter("@ReportingToId", details.ReportingToId),
                SqlHelper.CreateSqlParameter("@LocationId", details.Location),
                SqlHelper.CreateSqlParameter("@EmailId", details.EmailId),
                SqlHelper.CreateSqlParameter("@Remarks", details.Remarks),
                SqlHelper.CreateSqlParameter("@Billable", details.Billable),
                SqlHelper.CreateSqlParameter("@CteDoj", details.CteDoj),
                new SqlParameter("@EmpId", SqlDbType.Int) { Direction = ParameterDirection.Output },
                SqlHelper.CreateTvpParameter("@SkillIds", SqlHelper.CreateIdDataTable(details.TechnologySkill), "dbo.IdList"),
                SqlHelper.CreateTvpParameter("@ProjectIds", SqlHelper.CreateIdDataTable(details.ProjectAllocation), "dbo.IdList")
            };

            await SqlHelper.ExecuteStoredProcedureWithOutputAsync(_connectionString, "CreateResource1", parameters);

            return (int)parameters.First(p => p.ParameterName == "@EmpId").Value!;
        }

        public async Task<ResourceDetailsDto> GetResourceById(int empId)
        {
            var parameters = new List<SqlParameter>
            {
                SqlHelper.CreateSqlParameter("@EmpId", empId)
            };

            return await SqlHelper.DeserializeJsonFromReaderAsync<ResourceDetailsDto>(_connectionString, "GetResourceById1", parameters);
        }

        public async Task<bool> CheckEmailExists(string emailId)
        {
            var parameters = new List<SqlParameter>
            {
                SqlHelper.CreateSqlParameter("@EmailId", emailId),
                new SqlParameter("@DoesExist", SqlDbType.Bit) { Direction = ParameterDirection.Output }
            };

            await SqlHelper.ExecuteStoredProcedureWithOutputAsync(_connectionString, "CheckIfEmailExists", parameters);
            return (bool)parameters.First(p => p.ParameterName == "@DoesExist").Value!;
        }

        public async Task<IEnumerable<int>> GetResourceStatistics()
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Total_Resources", SqlDbType.Int) { Direction = ParameterDirection.Output },
                new SqlParameter("@Total_Billable", SqlDbType.Int) { Direction = ParameterDirection.Output },
                new SqlParameter("@Total_Projects", SqlDbType.Int) { Direction = ParameterDirection.Output }
            };

            await SqlHelper.ExecuteStoredProcedureWithOutputAsync(_connectionString, "GetResourceStatistics", parameters);

            return new List<int>
            {
                (int)parameters.First(p => p.ParameterName == "@Total_Resources").Value!,
                (int)parameters.First(p => p.ParameterName == "@Total_Billable").Value!,
                (int)parameters.First(p => p.ParameterName == "@Total_Projects").Value!
            };
        }

        public async Task UpdateResource(ResourceDto details)
        {
            var parameters = new List<SqlParameter>
    {
        SqlHelper.CreateSqlParameter("@EmpId", details.EmpId),
        SqlHelper.CreateSqlParameter("@ResourceName", details.ResourceName),
        SqlHelper.CreateSqlParameter("@DesignationId", details.Designation),
        SqlHelper.CreateSqlParameter("@ReportingToId", details.ReportingToId),
        SqlHelper.CreateSqlParameter("@LocationId", details.Location),
        SqlHelper.CreateSqlParameter("@EmailId", details.EmailId),
        SqlHelper.CreateSqlParameter("@Remarks", details.Remarks),
        SqlHelper.CreateSqlParameter("@Billable", details.Billable),
        SqlHelper.CreateSqlParameter("@CteDoj", details.CteDoj),
        SqlHelper.CreateTvpParameter("@SkillIds", SqlHelper.CreateIdDataTable(details.TechnologySkill), "dbo.IdList"),
        SqlHelper.CreateTvpParameter("@ProjectIds", SqlHelper.CreateIdDataTable(details.ProjectAllocation), "dbo.IdList")
    };

            await SqlHelper.ExecuteNonQueryAsync(_connectionString, "UpdateResource1", parameters);
        }

        public async Task DeleteResource(int empId)
        {
            var parameters = new List<SqlParameter>
    {
        SqlHelper.CreateSqlParameter("@EmpId", empId)
    };

            await SqlHelper.ExecuteNonQueryAsync(_connectionString, "DeleteResourceById1", parameters);
        }
        public async Task DeleteResourcesByEmpIdList(IEnumerable<int> empIds)
        {
            var tvp = SqlHelper.CreateIdDataTable(empIds, "EmpId");
            var parameters = new List<SqlParameter>
    {
        SqlHelper.CreateTvpParameter("@EmpIds", tvp, "EmpIdList")
    };

            await SqlHelper.ExecuteNonQueryAsync(_connectionString, "DeleteResourcesByEmpIdList1", parameters);
        }

        public async Task<DropdownResponseDto> GetDropdownData()
        {
            var result = await SqlHelper.DeserializeJsonFromReaderAsync<DropdownResponseDto>(_connectionString, "GetDropdownDataJson1");

            return result ?? new DropdownResponseDto();
        }

        public async Task<int> BulkUpdateResources(BulkEditDto bulkEditDetails)
        {
            var parameters = new List<SqlParameter>
    {
        SqlHelper.CreateTvpParameter("@ResourceIds", SqlHelper.CreateIdDataTable(bulkEditDetails.ResourceIds), "dbo.IdList"),
        SqlHelper.CreateSqlParameter("@DesignationId", bulkEditDetails.FeildsToEdit.DesignationId),
        SqlHelper.CreateSqlParameter("@LocationId", bulkEditDetails.FeildsToEdit.LocationId),
        SqlHelper.CreateSqlParameter("@ReportingToId", bulkEditDetails.FeildsToEdit.ReportingToId),
        SqlHelper.CreateSqlParameter("@Billable", bulkEditDetails.FeildsToEdit.Billable),
        SqlHelper.CreateSqlParameter("@CteDoj", bulkEditDetails.FeildsToEdit.CteDoj?.ToDateTime(TimeOnly.MinValue)),
        SqlHelper.CreateSqlParameter("@Remarks", bulkEditDetails.FeildsToEdit.Remarks),
        SqlHelper.CreateTvpParameter("@SkillIds", SqlHelper.CreateIdDataTable(bulkEditDetails.FeildsToEdit.SkillIds ?? new List<int>()), "dbo.IdList"),
        SqlHelper.CreateTvpParameter("@ProjectIds", SqlHelper.CreateIdDataTable(bulkEditDetails.FeildsToEdit.ProjectIds ?? new List<int>()), "dbo.IdList")
    };

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("BulkUpdateResources1", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.AddRange(parameters.ToArray());

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return reader.GetInt32(0);
            }

            return 0;
        }

        public async Task BulkCreateResourcesAsync(List<ResourceDto> resources)
        {
            var employeesTable = new DataTable();
            employeesTable.Columns.Add("TempKey", typeof(Guid));
            employeesTable.Columns.Add("ResourceName", typeof(string));
            employeesTable.Columns.Add("DesignationId", typeof(int));
            employeesTable.Columns.Add("ReportingToId", typeof(int));
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
                    resource.ReportingToId,
                    resource.Billable,
                    resource.Location,
                    resource.EmailId,
                    resource.CteDoj.ToDateTime(TimeOnly.MinValue),
                    resource.Remarks ?? string.Empty
                );

                foreach (var skillId in resource.TechnologySkill ?? Enumerable.Empty<int>())
                    skillsTable.Rows.Add(tempKey, skillId);

                foreach (var projectId in resource.ProjectAllocation ?? Enumerable.Empty<int>())
                    projectsTable.Rows.Add(tempKey, projectId);
            }

            var parameters = new List<SqlParameter>
    {
        SqlHelper.CreateTvpParameter("@Employees", employeesTable, "dbo.TempEmployee1"),
        SqlHelper.CreateTvpParameter("@Skills", skillsTable, "dbo.TempEmployeeSkills"),
        SqlHelper.CreateTvpParameter("@Projects", projectsTable, "dbo.TempProjectAllocation")
    };

            await SqlHelper.ExecuteNonQueryAsync(_connectionString, "sp_BulkImport2", parameters);
        }

        public async Task<List<OptionDto>?> GetRoleOptionsDropDownAsync()
        {
            using var conn = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_GetRoleOptions", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            var outputParam = new SqlParameter("@result", SqlDbType.NVarChar, -1)
            {
                Direction = ParameterDirection.Output
            };
            command.Parameters.Add(outputParam);

            await conn.OpenAsync();
            await command.ExecuteNonQueryAsync();

            var json = outputParam.Value as string;

            return !string.IsNullOrWhiteSpace(json)
                ? JsonSerializer.Deserialize<List<OptionDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                : new List<OptionDto>();
        }


    }
}
