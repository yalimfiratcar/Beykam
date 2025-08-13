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
    [Route("api/employer/jobposting")]
    public class JobPostingController : ControllerBase
    {
        private readonly AppDbContext _context;

        public JobPostingController(AppDbContext context)
        {
            _context = context;
        }

        // İşverein kendi ilanlarını görmek için
        [HttpGet("myjobpostings")]
        public async Task<IActionResult> GetMyJobPostings()
        {
            var employerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (employerIdClaim == null)
                return Unauthorized("Kullanıcı doğrulanamadı.");

            int employerId = int.Parse(employerIdClaim.Value);

            var jobPostings = await _context.JobPostings
                .Where(j => j.EmployerId == employerId)
                .Include(j => j.Applications)
                .ToListAsync();

            return Ok(jobPostings);
        }

        // yeni iş ilanı için
        [HttpPost("create")]
        public async Task<IActionResult> CreateJobPosting(CreateJobPostingDto dto)
        {
            var employerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (employerIdClaim == null)
                return Unauthorized("Kullanıcı doğrulanamadı.");

            int employerId = int.Parse(employerIdClaim.Value);

            var jobPosting = new JobPosting
            {
                EmployerId = employerId,
                Title = dto.Title,
                Description = dto.Description,
                EmploymentType = dto.EmploymentType,
                SalaryRange = dto.SalaryRange,
                ExperienceLevel = dto.ExperienceLevel,
                ApplicationDeadline = dto.ApplicationDeadline,
                CreatedAt = System.DateTime.UtcNow
            };

            _context.JobPostings.Add(jobPosting);
            await _context.SaveChangesAsync();

            return Ok(new { message = "İş ilanı başarıyla oluşturuldu.", jobPostingId = jobPosting.Id });
        }

        // iş ilanı güncelleme için
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateJobPosting(int id, UpdateJobPostingDto dto)
        {
            var employerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (employerIdClaim == null)
                return Unauthorized("Kullanıcı doğrulanamadı.");

            int employerId = int.Parse(employerIdClaim.Value);

            var jobPosting = await _context.JobPostings.FindAsync(id);

            if (jobPosting == null)
                return NotFound("İş ilanı bulunamadı.");

            if (jobPosting.EmployerId != employerId)
                return Forbid("Bu iş ilanını düzenleme yetkiniz yok.");

            jobPosting.Title = dto.Title;
            jobPosting.Description = dto.Description;
            jobPosting.EmploymentType = dto.EmploymentType;
            jobPosting.SalaryRange = dto.SalaryRange;
            jobPosting.ExperienceLevel = dto.ExperienceLevel;
            jobPosting.ApplicationDeadline = dto.ApplicationDeadline;

            await _context.SaveChangesAsync();

            return Ok("İş ilanı başarıyla güncellendi.");
        }

        // iş ilanını silme için
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJobPosting(int id)
        {
            var employerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (employerIdClaim == null)
                return Unauthorized("Kullanıcı doğrulanamadı.");

            int employerId = int.Parse(employerIdClaim.Value);

            var jobPosting = await _context.JobPostings.FindAsync(id);

            if (jobPosting == null)
                return NotFound("İş ilanı bulunamadı.");

            if (jobPosting.EmployerId != employerId)
                return Forbid("Bu iş ilanını silme yetkiniz yok.");

            _context.JobPostings.Remove(jobPosting);
            await _context.SaveChangesAsync();

            return Ok("İş ilanı başarıyla silindi.");
        }

        // iş ilanına yapılan başvurular için
        [HttpGet("{jobPostingId}/applications")]
        public async Task<IActionResult> GetApplicationsForJobPosting(int jobPostingId)
        {
            var employerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (employerIdClaim == null)
                return Unauthorized("Kullanıcı doğrulanamadı.");

            int employerId = int.Parse(employerIdClaim.Value);

            var jobPosting = await _context.JobPostings
                .Include(j => j.Applications)
                .ThenInclude(a => a.JobSeeker)
                .FirstOrDefaultAsync(j => j.Id == jobPostingId);

            if (jobPosting == null)
                return NotFound("İş ilanı bulunamadı.");

            if (jobPosting.EmployerId != employerId)
                return Forbid("Bu iş ilanına erişim yetkiniz yok.");

            var applications = jobPosting.Applications.Select(a => new
            {
                a.Id,
                a.AppliedAt,
                a.Status,
                JobSeeker = new
                {
                    a.JobSeeker.Id,
                    a.JobSeeker.FirstName,
                    a.JobSeeker.Email,
                    
                }
            });

            return Ok(applications);
        }
    }
}
