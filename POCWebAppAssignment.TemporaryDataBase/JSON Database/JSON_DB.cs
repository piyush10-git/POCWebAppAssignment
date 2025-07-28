using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using POCWebAppAssignment.Model;

namespace POCWebAppAssignment.TemporaryDatabase.JSON_Database
{
    internal static class JSON_DB
    {
        private static string _JSONFilePath = "./DB.JSON";
        public static void SaveData(List<Resource> data)
        {
            string JsonString = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_JSONFilePath, JsonString);
        }

        public static List<Resource> GetData()
        {
            var content = File.ReadAllText(_JSONFilePath);
            return JsonSerializer.Deserialize<List<Resource>>(content);
        }
    }
}
