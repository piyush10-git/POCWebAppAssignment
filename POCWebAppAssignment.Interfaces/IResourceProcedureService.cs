using POCWebAppAssignment.Model;
using POCWebAppAssignment.Model.DTOs;

namespace POCWebAppAssignment.Interfaces
{
    public interface IResourceProcedureService
    {
        Task BulkCreateResourcesAsync(List<ResourceDto> resources);
        Task<int> BulkUpdateResources(BulkEditDto bulkEditDetails);
        Task<bool> CheckEmailExists(string emailId);
        Task<int> CreateResource(ResourceDto details);
        Task DeleteResource(int empId);
        Task DeleteResourcesByEmpIdList(IEnumerable<int> empIds);
        Task<IEnumerable<Resource>> GetAll();
        Task<DropdownResponseDto> GetDropdownData();
        Task<ResourceDetailsDto> GetResourceById(int empId);
        Task<IEnumerable<int>> GetResourceStatistics();
        Task<List<OptionDto>?> GetRoleOptionsDropDownAsync();
        Task UpdateResource(ResourceDto details);
    }
}