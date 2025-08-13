using Microsoft.AspNetCore.Mvc;
using BeykamBackend.Data;
using BeykamBackend.DTOs;
using BeykamBackend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;

namespace BeykamBackend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/jobseeker")]
    public class JobSeekerController : ControllerBase
    {
        private readonly AppDbContext _context;

        public JobSeekerController(AppDbContext context)
        {
            _context = context;
        }

        // Tüm iş ilanlarını detaylarıyla listele
        [HttpGet("jobpostings")]
        public async Task<IActionResult> GetAllJobPostings()
        {
            var jobPostings = await _context.JobPostings
                .Include(j => j.Employer)
                .Include(j => j.Applications) // Başvuruların sayısını göstermek için
                .ToListAsync();

            var result = jobPostings.Select(j => new 
            {
                j.Id,
                j.Title,
                j.Description,
                j.EmploymentType,
                j.SalaryRange,
                j.ExperienceLevel,
                j.ApplicationDeadline,
                j.CreatedAt,
                Employer = new 
                {
                    j.Employer.Id,
                    j.Employer.CompanyName,
                    j.Employer.Email,
                    j.Employer.TaxNumber
                },
                ApplicationCount = j.Applications.Count
            });

            return Ok(result);
        }

        // İlanlara başvuru yap
        [HttpPost("apply")]
        public async Task<IActionResult> ApplyToJob(ApplyJobDto dto)
        {
            var jobSeekerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (jobSeekerIdClaim == null)
                return Unauthorized("Kullanıcı doğrulanamadı.");

            int jobSeekerId = int.Parse(jobSeekerIdClaim.Value);

            // Zaten başvurdu mu kontrol et
            var existingApplication = await _context.Applications
                .FirstOrDefaultAsync(a => a.JobPostingId == dto.JobPostingId && a.JobSeekerId == jobSeekerId);

            if (existingApplication != null)
                return BadRequest("Zaten bu ilana başvurdunuz.");

            var application = new Application
            {
                JobPostingId = dto.JobPostingId,
                JobSeekerId = jobSeekerId,
                AppliedAt = System.DateTime.UtcNow,
                Status = "Pending"
            };

            _context.Applications.Add(application);
            await _context.SaveChangesAsync();

            return Ok("Başvurunuz başarıyla alındı.");
        }

        // Kendi başvurularını gör
        [HttpGet("myapplications")]
        public async Task<IActionResult> GetMyApplications()
        {
            var jobSeekerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (jobSeekerIdClaim == null)
                return Unauthorized("Kullanıcı doğrulanamadı.");

            int jobSeekerId = int.Parse(jobSeekerIdClaim.Value);

            var applications = await _context.Applications
                .Where(a => a.JobSeekerId == jobSeekerId)
                .Include(a => a.JobPosting)
                .ThenInclude(j => j.Employer)
                .ToListAsync();

            var result = applications.Select(a => new 
            {
                a.Id,
                a.AppliedAt,
                a.Status,
                JobPosting = new
                {
                    a.JobPosting.Id,
                    a.JobPosting.Title,
                    a.JobPosting.Description,
                    a.JobPosting.EmploymentType,
                    a.JobPosting.SalaryRange,
                    a.JobPosting.ExperienceLevel,
                    a.JobPosting.ApplicationDeadline,
                    Employer = new
                    {
                        a.JobPosting.Employer.Id,
                        a.JobPosting.Employer.CompanyName
                    }
                }
            });

            return Ok(result);
        }
    }
}
