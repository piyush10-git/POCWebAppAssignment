namespace POCWebAppAssignment.Model.DTOs
{
    public class BulkEditDto
    {
        public List<int> ResourceIds { get; set; }
        public FeildsToEdit FeildsToEdit { get; set; }
    }

    public class FeildsToEdit
    {
        public int? DesignationId { get; set; }
        public int? LocationId { get; set; }
        public string? ReportingTo { get; set; }
        public bool? Billable { get; set; }
        public DateOnly? CteDoj { get; set; }
        public string? Remarks { get; set; }

        public List<int>? SkillIds { get; set; }
        public List<int>? ProjectIds { get; set; }
    }

}
