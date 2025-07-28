namespace POCWebAppAssignment.Model
{
    public class Resource
    {
        public int? EmpId { get; set; }
        public string ResourceName { get; set; }
        public string Designation { get; set; }
        public string ReportingTo { get; set; }
        public bool Billable { get; set; }
        public string TechnologySkill { get; set; }
        public string ProjectAllocation { get; set; }
        public string Location { get; set; }
        public string EmailId { get; set; }
        public DateOnly CteDoj { get; set; }
        public string? Remarks { get; set; }
    }

}
