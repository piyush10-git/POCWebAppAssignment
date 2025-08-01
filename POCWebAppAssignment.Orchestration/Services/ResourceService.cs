using Microsoft.Extensions.Logging;
using POCWebAppAssignment.Interfaces;
using POCWebAppAssignment.Model;
using POCWebAppAssignment.Model.DTOs;
using POCWebAppAssignment.Repository.RunStoredProcedures;


namespace POCWebAppAssignment.Orchestration.Services
{
    public class ResourceService: IResourceService
    {
        private readonly IResourceRepository _resourceRepository;
        private readonly ILogger<ResourceService> _logger;
        public ResourceService(IResourceRepository newResourceRepository, ILogger<ResourceService> logger) {
            _resourceRepository = newResourceRepository;
            _logger = logger;
        }
        public async Task<IEnumerable<Resource>> GetAllResourcesAsync()
        {
            try
            {
                _logger.LogInformation("Fetching all resources.");
                return await _resourceRepository.GetAllResourcesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching all resources.");
                throw;
            }
        }
        public async Task<ResourceDetailsDto?> GetResourceByIdAsync(int empId)
        {
            try
            {
                _logger.LogInformation("Fetching resource with ID {EmpId}.", empId);
                return await _resourceRepository.GetResourceByIdAsync(empId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching resource with ID {EmpId}.", empId);
                throw;
            }
        }
        public async Task<int> CreateResourceAsync(ResourceDto resource)
        {
            try
            {
                _logger.LogInformation("Creating a new resource with Email {Email}.", resource.EmailId);
                return await _resourceRepository.CreateResourceAsync(resource);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating resource with Email {Email}.", resource.EmailId);
                throw;
            }
        }
        public async Task UpdateResourceAsync(ResourceDto resource)
        {
            try
            {
                _logger.LogInformation("Updating resource with ID {EmpId}.", resource.EmpId);
                await _resourceRepository.UpdateResourceAsync(resource);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating resource with ID {EmpId}.", resource.EmpId);
                throw;
            }
        }
        public async Task DeleteResourceAsync(int empId)
        {
            try
            {
                _logger.LogInformation("Deleting resource with ID {EmpId}.", empId);
                await _resourceRepository.DeleteResourceAsync(empId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting resource with ID {EmpId}.", empId);
                throw;
            }
        }

        public async Task DeleteResourcesByEmpIdListAsync(List<int> empIds)
        {
            try
            {
                _logger.LogInformation("Deleting resources with IDs {empIds}.", empIds.ToString());
                await _resourceRepository.DeleteResourcesByEmpIdListAsync(empIds);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting resource with IDs {empIds}.", empIds.ToString());
                throw;
            }
        }

        public async Task<IEnumerable<int>> GetResourceStatisticsAsync()
        {
            try
            {
                _logger.LogInformation("Fetching resource statistics.");
                return await _resourceRepository.GetResourceStatisticsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching resource statistics.");
                throw;
            }
        }

        public async Task<bool> CheckEmailExistsAsync(string emailId)
        {
            try
            {
                _logger.LogInformation("Checking if email exists: {Email}.", emailId);
                return await _resourceRepository.CheckEmailExistsAsync(emailId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking email existence: {Email}.", emailId);
                throw;
            }
        }

        public async Task<DropdownResponseDto> GetDropdownDataAsync()
        {
            try
            {
                _logger.LogInformation("Getting the drop down data.");
                return await _resourceRepository.GetDropdownDataAsync();
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
                return await _resourceRepository.BulkUpdateResourcesAsync(bulkEditDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error bulk updating resources with ID {EmpIds}.", bulkEditDetails.ResourceIds.ToString());
                throw;
            }
        }

        public async Task BulkCreateResourcesAsync(List<ResourceDto> resources)
        {
            try
            {
                _logger.LogInformation("Createing bulk resources.");
                await _resourceRepository.BulkCreateResourcesAsync(resources);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating {#resources} resources", resources.Count.ToString());
                throw;
            }
        }

        public async Task<List<OptionDto>?> GetRoleOptionsDropDownAsync()
        {
            return await _resourceRepository.GetRoleOptionsDropDownAsync();
        }
    }
}
