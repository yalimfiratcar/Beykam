using Microsoft.EntityFrameworkCore;
using BeykamBackend.Models;

namespace BeykamBackend.Data
{
    public class AppDbContext : DbContext
    {
        
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<JobSeeker> JobSeekers { get; set; }
        public DbSet<Employer> Employers { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<JobPosting> JobPostings { get; set;}
        public DbSet<Application> Applications { get; set; }

    }
}
