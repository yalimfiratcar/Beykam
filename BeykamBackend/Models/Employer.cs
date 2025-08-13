namespace BeykamBackend.Models
{
    public class Employer
    {
        public int Id { get; set; }
        public string TaxNumber { get; set; } = null!;
        public string CompanyName { get; set; } = null!;
        public string TaxOffice { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
    }
}
