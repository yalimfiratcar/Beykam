namespace BeykamBackend.Models
{
    public class Application
    {
        public int Id { get; set; }
        public int JobPostingId { get; set; }
        public int JobSeekerId { get; set; }
        public DateTime AppliedAt { get; set; }
        public string Status { get; set; } = "Pending";

        public JobPosting JobPosting { get; set; } = null!;
        public JobSeeker JobSeeker { get; set; } = null!;
    }
}
