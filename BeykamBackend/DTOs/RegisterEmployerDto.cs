namespace BeykamBackend.DTOs
{
    public class RegisterEmployerDto
    {
        public string TaxNumber { get; set; } = null!;
        public string CompanyName { get; set; } = null!;
        public string TaxOffice { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string ConfirmPassword { get; set; } = null!;
    }
}
