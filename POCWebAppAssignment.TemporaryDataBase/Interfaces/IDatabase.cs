using POCWebAppAssignment.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCWebAppAssignment.TemporaryDatabase.Interfaces
{
    public interface IDatabase
    {
        Task<IEnumerable<Resource>> GetAllResources();

        Task<Resource?> GetResourceById(int empId);

        Task AddResource(Resource newReource);

        Task UpdateResource(int empId, Resource updatedResource);

        Task DeleteResource(int empId);
    }
}
