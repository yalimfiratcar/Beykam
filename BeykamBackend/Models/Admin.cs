namespace BeykamBackend.Models
{
    public class Admin
    {
        public int Id { get; set; }
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
    }
}
