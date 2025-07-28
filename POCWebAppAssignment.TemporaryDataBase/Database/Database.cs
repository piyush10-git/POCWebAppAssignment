using POCWebAppAssignment.Model;
using POCWebAppAssignment.TemporaryDatabase.Interfaces;
using POCWebAppAssignment.TemporaryDatabase.JSON_Database;

namespace POCWebAppAssignment.TemporaryDatabase.Database
{
    public class Database : IDatabase
    {
        private List<Resource> _resourcesArray;
        public Database()
        {
            _resourcesArray = JSON_DB.GetData();
        }
        public async Task<Resource?> GetResourceById(int empId)
        {
            await Task.Delay(50);
            return _resourcesArray.FirstOrDefault(resource => resource.EmpId == empId);
        }

        public async Task<IEnumerable<Resource>> GetAllResources()
        {
            await Task.Delay(50);
            return _resourcesArray.ToList();
        }
        public async Task AddResource(Resource newReource)
        {
            await Task.Delay(50);
            _resourcesArray.Add(newReource);
            SaveData();
        }

        public async Task UpdateResource(int empId, Resource updatedResource)
        {
            int index = _resourcesArray.FindIndex(resource => resource.EmpId == empId);
            if (index != -1)
            {
                _resourcesArray[index] = updatedResource;
                SaveData();
            }
        }

        public async Task DeleteResource(int empId)
        {
            Resource resource = await GetResourceById(empId);
            if (resource != null)
            {
                bool removed = _resourcesArray.Remove(resource);
                if (removed)
                {
                    SaveData();
                }
            }
        }

        public void SaveData()
        {
            JSON_DB.SaveData(_resourcesArray);
        }

        public List<Resource> GetData()
        {
            return JSON_DB.GetData();
        }
    }
}
