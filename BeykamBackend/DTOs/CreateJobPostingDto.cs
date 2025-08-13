namespace BeykamBackend.DTOs
{
    public class CreateJobPostingDto
    {
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string EmploymentType { get; set; } = null!; // Tam zamanlı, partime vb
        public string SalaryRange { get; set; } = null!;   // maaş aralığı
        public string ExperienceLevel { get; set; } = null!; // deneyim
        public DateTime ApplicationDeadline { get; set; }  // son başvuru tarihi
    }
}
