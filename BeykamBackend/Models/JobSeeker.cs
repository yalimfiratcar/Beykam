namespace BeykamBackend.Models
{
    public class JobSeeker
    {
        public int Id { get; set; }
        public string TcKimlikNo { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public DateTime BirthDate { get; set; }
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
    }
}
