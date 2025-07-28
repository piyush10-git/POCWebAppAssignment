using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCWebAppAssignment.Model.DTOs
{
    public class ResourceDto
    {
        public int? EmpId { get; set; }
        public string ResourceName { get; set; }
        public int Designation { get; set; }
        public string ReportingTo { get; set; }
        public bool Billable { get; set; }
        public int Location { get; set; }
        public string EmailId { get; set; }
        public DateOnly CteDoj { get; set; }
        public string Remarks { get; set; }

        public List<int> TechnologySkill { get; set; }
        public List<int> ProjectAllocation { get; set; }
    }

}
