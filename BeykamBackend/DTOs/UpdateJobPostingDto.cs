namespace BeykamBackend.DTOs
{
    public class UpdateJobPostingDto
    {
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string EmploymentType { get; set; } = null!;
        public string SalaryRange { get; set; } = null!;
        public string ExperienceLevel { get; set; } = null!;
        public DateTime ApplicationDeadline { get; set; }
    }
}
