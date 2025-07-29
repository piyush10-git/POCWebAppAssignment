using POCWebAppAssignment.Model;

namespace POCWebAppAssignment.Interfaces
{
    public interface IRunStoredProcedures
    {
        Task<IEnumerable<Resource>> GetAll();

        Task<int> CreateResource(Resource details);

        Task<Resource> GetResourceById(int empId);

        Task UpdateResource(Resource details);

        Task DeleteResource(int empId);

        Task<IEnumerable<int>> GetResourceStatistics();
        Task<bool> CheckEmailExists(string emailId);
        Task DeleteResourcesByEmpIdList(IEnumerable<int> empIds);
        //Task BulkCreateResourcesAsync(IEnumerable<Resource> resources);
    }
}
