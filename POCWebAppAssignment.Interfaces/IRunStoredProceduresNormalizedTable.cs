using POCWebAppAssignment.Model;
using POCWebAppAssignment.Model.DTOs;


namespace POCWebAppAssignment.Interfaces
{
    public interface IRunStoredProceduresNormalizedTable
    {
        Task<IEnumerable<Resource>> GetAll();

        Task<int> CreateResource(ResourceDto details);
        Task<ResourceDetailsDto> GetResourceById(int empId);
        Task UpdateResource(ResourceDto details);
        Task DeleteResource(int empId);
        Task<IEnumerable<int>> GetResourceStatistics();
        Task<bool> CheckEmailExists(string emailId);
        Task DeleteResourcesByEmpIdList(IEnumerable<int> empIds);
        Task<DropdownResponseDto> GetDropdownData();
        Task<int> BulkUpdateResources(BulkEditDto bulkEditDetails);
        Task BulkCreateResourcesAsync(List<ResourceDto> resources);

    }
}
