// İş veren login DTO'su
namespace BeykamBackend.DTOs
{
    public class EmployerLoginDto
    {
        public string TaxNumber { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
