using Microsoft.Extensions.Logging;
using POCWebAppAssignment.Interfaces;
using POCWebAppAssignment.Model;
using POCWebAppAssignment.Model.DTOs;


namespace POCWebAppAssignment.Repository.Repositories
{
    public class ResourceRepository : IResourceRepository
    {
        private readonly IRunStoredProceduresNormalizedTable _runStoredProcedures;
        private readonly ILogger<ResourceRepository> _logger;
        public ResourceRepository(IRunStoredProceduresNormalizedTable runStoredProcedures, ILogger<ResourceRepository> logger) {
            _runStoredProcedures = runStoredProcedures;
            _logger = logger;
        }

        public async Task<int> CreateResourceAsync(ResourceDto resource)
        {
            try
            {
                _logger.LogInformation("Creating resource with Email {Email}.", resource.EmailId);
                return await _runStoredProcedures.CreateResource(resource);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating resource with Email {Email}.", resource.EmailId);
                throw;
            }
        }

        public async Task DeleteResourceAsync(int empId)
        {
            try
            {
                _logger.LogInformation("Deleting resource with ID {EmpId}.", empId);
                await _runStoredProcedures.DeleteResource(empId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting resource with ID {EmpId}.", empId);
                throw;
            }
        }

        public async Task DeleteResourcesByEmpIdListAsync(List<int> empIds)
        {
            try
            {
                _logger.LogInformation("Deleting resources with IDs {empIds}.", empIds.ToString());
                await _runStoredProcedures.DeleteResourcesByEmpIdList(empIds);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting resource with IDs {empIds}.", empIds.ToString());
                throw;
            }
        }

        public async Task<IEnumerable<Resource>> GetAllResourcesAsync()
        {
            try
            {
                _logger.LogInformation("Retrieving all resources.");
                return await _runStoredProcedures.GetAll();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all resources.");
                throw;
            }
        }

        public async Task<ResourceDetailsDto?> GetResourceByIdAsync(int empId)
        {
            try
            {
                _logger.LogInformation("Retrieving resource with ID {EmpId}.", empId);
                return await _runStoredProcedures.GetResourceById(empId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving resource with ID {EmpId}.", empId);
                throw;
            }
        }

        public async Task UpdateResourceAsync(ResourceDto resource)
        {
            try
            {
                _logger.LogInformation("Updating resource with ID {EmpId}.", resource.EmpId);
                await _runStoredProcedures.UpdateResource(resource);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating resource with ID {EmpId}.", resource.EmpId);
                throw;
            }
        }

        public async Task<IEnumerable<int>> GetResourceStatisticsAsync()
        {
            try
            {
                _logger.LogInformation("Retrieving resource statistics.");
                return await _runStoredProcedures.GetResourceStatistics();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving resource statistics.");
                throw;
            }
        }

        public async Task<bool> CheckEmailExistsAsync(string emailId)
        {
            try
            {
                _logger.LogInformation("Checking if email exists: {Email}.", emailId);
                return await _runStoredProcedures.CheckEmailExists(emailId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if email exists: {Email}.", emailId);
                throw;
            }
        }

        public async Task<DropdownResponseDto> GetDropdownDataAsync()
        {
            try
            {
                _logger.LogInformation("Getting the drop down data.");
                return await _runStoredProcedures.GetDropdownData();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting drop down data.");
                throw;
            }
        }

        public async Task<int> BulkUpdateResourcesAsync(BulkEditDto bulkEditDetails)
        {
            try
            {
                _logger.LogInformation("Bulk Updating resource with IDs {EmpIds}.", bulkEditDetails.ResourceIds.ToString());
                return await _runStoredProcedures.BulkUpdateResources(bulkEditDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error bulk updating resources with ID {EmpIds}.", bulkEditDetails.ResourceIds.ToString());
                throw;
            }
        }

    }
}
