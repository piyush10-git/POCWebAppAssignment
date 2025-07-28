using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using POCWebAppAssignment.Model;
using POCWebAppAssignment.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace POCWebAppAssignment.Repository.RunStoredProcedures
{
    public class RunStoredProcedures: IRunStoredProcedures
    {
        private readonly string _connectionString;
        public RunStoredProcedures(IConfiguration configuration) {
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

        public async Task<int> CreateResource(Resource details)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("AddResource", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@ResourceName", details.ResourceName);
                    command.Parameters.AddWithValue("@Designation", details.Designation);
                    command.Parameters.AddWithValue("@ReportingTo", details.ReportingTo);
                    command.Parameters.AddWithValue("@TechnologySkill", details.TechnologySkill);
                    command.Parameters.AddWithValue("@ProjectAllocation", details.ProjectAllocation);
                    command.Parameters.AddWithValue("@Location", details.Location);
                    command.Parameters.AddWithValue("@EmailId", details.EmailId);
                    command.Parameters.AddWithValue("@Remarks", details.Remarks);
                    command.Parameters.AddWithValue("@Billable", details.Billable);
                    command.Parameters.AddWithValue("@CteDoj", details.CteDoj);

                    // Add OUTPUT parameter for EmpId
                    SqlParameter outputIdParam = new SqlParameter("@EmpId", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(outputIdParam);

                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();

                    // Retrieve the output value
                    return (int)outputIdParam.Value;
                }
            }
        }


        public async Task<Resource> GetResourceById(int empId)
        {
            Resource details = null;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("GetResourceById", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@EmpId", empId);

                    connection.Open();
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (reader.Read())
                        {
                            details = new Resource
                            {
                                EmpId = (int)reader["EmpId"],
                                ResourceName = reader["ResourceName"].ToString(),
                                Designation = reader["Designation"].ToString(),
                                ReportingTo = reader["ReportingTo"].ToString(),
                                TechnologySkill = reader["TechnologySkill"].ToString(),
                                ProjectAllocation = reader["ProjectAllocation"].ToString(),
                                Location = reader["Location"].ToString(),
                                EmailId = reader["EmailId"].ToString(),
                                Remarks = reader["Remarks"].ToString(),
                                Billable = Convert.ToBoolean(reader["Billable"]),
                                CteDoj = DateOnly.FromDateTime(Convert.ToDateTime(reader["CteDoj"]))
                            };
                        }
                    }
                }
            }

            return details;
        }

        public async Task UpdateResource(Resource details)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("UpdateResource", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@EmpId", details.EmpId);
                    command.Parameters.AddWithValue("@ResourceName", details.ResourceName);
                    command.Parameters.AddWithValue("@Designation", details.Designation);
                    command.Parameters.AddWithValue("@ReportingTo", details.ReportingTo);
                    command.Parameters.AddWithValue("@TechnologySkill", details.TechnologySkill);
                    command.Parameters.AddWithValue("@ProjectAllocation", details.ProjectAllocation);
                    command.Parameters.AddWithValue("@Location", details.Location);
                    command.Parameters.AddWithValue("@EmailId", details.EmailId);
                    command.Parameters.AddWithValue("@Remarks", details.Remarks);
                    command.Parameters.AddWithValue("@Billable", details.Billable);
                    command.Parameters.AddWithValue("@CteDoj", details.CteDoj);

                    connection.Open();
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task DeleteResource(int empId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("DeleteResource", connection))
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

    }
}
