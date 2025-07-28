namespace POCWebAppAssignment.Model.DTOs
{
    public class ResourceDetailsDto
    {
        public int EmpId { get; set; }
        public string Name { get; set; }
        public string ReportingTo { get; set; }
        public bool Billable { get; set; }
        public string Email { get; set; }
        public string Remarks { get; set; }
        public DateOnly CteDoj { get; set; }

        public OptionDto Designation { get; set; }
        public OptionDto Location { get; set; }

        public List<OptionDto> Skills { get; set; }
        public List<OptionDto> Projects { get; set; }
    }

}
