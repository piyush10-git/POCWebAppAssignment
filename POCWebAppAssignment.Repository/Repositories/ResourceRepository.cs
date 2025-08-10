using Microsoft.Extensions.Logging;
using POCWebAppAssignment.Interfaces;
using POCWebAppAssignment.Model;
using POCWebAppAssignment.Model.DTOs;

namespace POCWebAppAssignment.Repository.Repositories
{
    public class ResourceRepository : IResourceRepository
    {
        private readonly IResourceProcedureService _runStoredProcedures;
        private readonly ILogger<ResourceRepository> _logger;

        public ResourceRepository(IResourceProcedureService runStoredProcedures, ILogger<ResourceRepository> logger)
        {
            _runStoredProcedures = runStoredProcedures;
            _logger = logger;
        }

        public async Task<int> CreateResourceAsync(ResourceDto resource)
        {
            try
            {
                _logger.LogInformation("Creating resource with email: {Email}.", resource.EmailId);
                var id = await _runStoredProcedures.CreateResource(resource);
                _logger.LogInformation("Successfully created resource with ID: {EmpId}.", id);
                return id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create resource with email: {Email}.", resource.EmailId);
                throw;
            }
        }

        public async Task DeleteResourceAsync(int empId)
        {
            try
            {
                _logger.LogInformation("Deleting resource with ID: {EmpId}.", empId);
                await _runStoredProcedures.DeleteResource(empId);
                _logger.LogInformation("Successfully deleted resource with ID: {EmpId}.", empId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete resource with ID: {EmpId}.", empId);
                throw;
            }
        }

        public async Task DeleteResourcesByEmpIdListAsync(List<int> empIds)
        {
            try
            {
                _logger.LogInformation("Deleting multiple resources. IDs: {@EmpIds}", empIds);
                await _runStoredProcedures.DeleteResourcesByEmpIdList(empIds);
                _logger.LogInformation("Successfully deleted {Count} resources.", empIds.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting multiple resources. IDs: {@EmpIds}", empIds);
                throw;
            }
        }

        public async Task<IEnumerable<Resource>> GetAllResourcesAsync()
        {
            try
            {
                _logger.LogInformation("Fetching all resources.");
                var resources = await _runStoredProcedures.GetAll();
                _logger.LogInformation("Successfully retrieved {Count} resources.", resources?.Count() ?? 0);
                return resources;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all resources.");
                throw;
            }
        }

        public async Task<ResourceDetailsDto?> GetResourceByIdAsync(int empId)
        {
            try
            {
                _logger.LogInformation("Fetching resource with ID: {EmpId}.", empId);
                var resource = await _runStoredProcedures.GetResourceById(empId);
                if (resource == null)
                {
                    _logger.LogWarning("No resource found with ID: {EmpId}.", empId);
                }
                else
                {
                    _logger.LogInformation("Successfully retrieved resource with ID: {EmpId}.", empId);
                }
                return resource;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving resource with ID: {EmpId}.", empId);
                throw;
            }
        }

        public async Task UpdateResourceAsync(ResourceDto resource)
        {
            try
            {
                _logger.LogInformation("Updating resource with ID: {EmpId}.", resource.EmpId);
                await _runStoredProcedures.UpdateResource(resource);
                _logger.LogInformation("Successfully updated resource with ID: {EmpId}.", resource.EmpId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating resource with ID: {EmpId}.", resource.EmpId);
                throw;
            }
        }

        public async Task<IEnumerable<int>> GetResourceStatisticsAsync()
        {
            try
            {
                _logger.LogInformation("Fetching resource statistics.");
                var stats = await _runStoredProcedures.GetResourceStatistics();
                _logger.LogInformation("Successfully retrieved resource statistics.");
                return stats;
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
                var exists = await _runStoredProcedures.CheckEmailExists(emailId);

                _logger.LogInformation("Email check result for {Email}: {Exists}.", emailId, exists);
                return exists;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking email existence: {Email}.", emailId);
                throw;
            }
        }

        public async Task<DropdownResponseDto> GetDropdownDataAsync()
        {
            try
            {
                _logger.LogInformation("Fetching dropdown data.");
                var data = await _runStoredProcedures.GetDropdownData();
                _logger.LogInformation("Successfully fetched dropdown data.");
                return data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching dropdown data.");
                throw;
            }
        }

        public async Task<int> BulkUpdateResourcesAsync(BulkEditDto bulkEditDetails)
        {
            try
            {
                _logger.LogInformation("Bulk updating resources. IDs: {@EmpIds}", bulkEditDetails.ResourceIds);
                var updated = await _runStoredProcedures.BulkUpdateResources(bulkEditDetails);
                _logger.LogInformation("Successfully bulk updated {Count} resources.", updated);
                return updated;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during bulk update of resources. IDs: {@EmpIds}", bulkEditDetails.ResourceIds);
                throw;
            }
        }

        public async Task BulkCreateResourcesAsync(List<ResourceDto> resources)
        {
            try
            {
                _logger.LogInformation("Creating {Count} resources in bulk.", resources.Count);
                await _runStoredProcedures.BulkCreateResourcesAsync(resources);
                _logger.LogInformation("Successfully created {Count} resources in bulk.", resources.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during bulk creation of {Count} resources.", resources.Count);
                throw;
            }
        }

        public async Task<List<OptionDto>?> GetRoleOptionsDropDownAsync()
        {
            try
            {
                _logger.LogInformation("Fetching role options for dropdown.");
                var options = await _runStoredProcedures.GetRoleOptionsDropDownAsync();
                _logger.LogInformation("Successfully retrieved {Count} role options.", options?.Count ?? 0);
                return options;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching role options for dropdown.");
                throw;
            }
        }
    }
}
