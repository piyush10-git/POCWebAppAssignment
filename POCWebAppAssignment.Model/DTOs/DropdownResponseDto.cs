namespace POCWebAppAssignment.Model.DTOs
{
    public class DropdownResponseDto
    {
        public List<OptionDto> Designations { get; set; }
        public List<OptionDto> Skills { get; set; }
        public List<OptionDto> Locations { get; set; }
        public List<OptionDto> Projects { get; set; }
        public List<OptionDto> Managers { get; set; }
    }
}
