namespace BeykamBackend.Models
{
    public class JobPosting
    {
        public int Id { get; set; }
        public int EmployerId { get; set; }           // İlanı oluşturan işverenin Id'si 
        public string Title { get; set; } = null!;    
        public string Description { get; set; } = null!;  
        public string EmploymentType { get; set; } = null!;  
        public string SalaryRange { get; set; } = null!;      
        public string ExperienceLevel { get; set; } = null!;  
        public DateTime ApplicationDeadline { get; set; }    
        public DateTime CreatedAt { get; set; }       

        public Employer Employer { get; set; } = null!;  // İlan veren işveren bilgisi 
        public ICollection<Application> Applications { get; set; } = new List<Application>(); // Başvurular
    }
}
