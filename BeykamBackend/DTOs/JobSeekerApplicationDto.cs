namespace BeykamBackend.DTOs
{
    public class JobSeekerApplicationDto
    {
        public int ApplicationId { get; set; }
        public DateTime AppliedAt { get; set; }
        public string Status { get; set; } = null!;

        // İş ilanı bilgileri
        public int JobPostingId { get; set; }
        public string JobTitle { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string EmploymentType { get; set; } = null!;
        public string SalaryRange { get; set; } = null!;
        public string ExperienceLevel { get; set; } = null!;
        public DateTime ApplicationDeadline { get; set; }

        // İşveren bilgileri
        public int EmployerId { get; set; }
        public string CompanyName { get; set; } = null!;
        public string Email { get; set; } = null!;
    }
}
