using POCWebAppAssignment.Model;
using POCWebAppAssignment.Model.DTOs;

namespace POCWebAppAssignment.Interfaces
{
    public interface IResourceService
    {
        Task<IEnumerable<Resource>> GetAllResourcesAsync();
        Task<ResourceDetailsDto?> GetResourceByIdAsync(int empId);
        Task<int> CreateResourceAsync(ResourceDto resource);
        Task UpdateResourceAsync(ResourceDto resource);
        Task DeleteResourceAsync(int empId);
        Task<IEnumerable<int>> GetResourceStatisticsAsync();
        Task<bool> CheckEmailExistsAsync(string emailId);
        Task DeleteResourcesByEmpIdListAsync(List<int> empIds);
        Task<DropdownResponseDto> GetDropdownDataAsync();
        Task<int> BulkUpdateResourcesAsync(BulkEditDto bulkEditDetails);
        Task BulkCreateResourcesAsync(List<ResourceDto> resources);

    }
}
