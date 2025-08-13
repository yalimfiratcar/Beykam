using Microsoft.AspNetCore.Mvc;
using BeykamBackend.Data;           
using BeykamBackend.Models;         
using System.Security.Cryptography; 
using System.Text;                  
using System.Threading.Tasks;      
using Microsoft.EntityFrameworkCore; 
using BeykamBackend.DTOs;           
using Microsoft.IdentityModel.Tokens; 
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;      
using Microsoft.Extensions.Configuration; 
using System;

namespace BeykamBackend.Controllers
{
    [ApiController]
    [Route("api/jobseeker/auth")]  
    public class JobSeekerAuthController : ControllerBase
    {
        private readonly AppDbContext _context;     
        private readonly IConfiguration _config;    

        public JobSeekerAuthController(AppDbContext context, IConfiguration config)
        {
            _context = context;                      
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterJobSeekerDto dto)
        {
            
            if (dto.Password != dto.ConfirmPassword)
                return BadRequest("Parolalar eşleşmiyor.");

            
            if (await _context.JobSeekers.AnyAsync(j => j.Email == dto.Email))
                return BadRequest("Bu email zaten kayıtlı.");

            // parola hashleme
            var passwordHash = HashPassword(dto.Password);

            // yeni iş arayan oluştrma
            var jobSeeker = new JobSeeker
            {
                TcKimlikNo = dto.TcKimlikNo,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                BirthDate = dto.BirthDate,
                Email = dto.Email,
                PasswordHash = passwordHash
            };

            
            _context.JobSeekers.Add(jobSeeker);
            await _context.SaveChangesAsync();

            
            return Ok("Kayıt başarılı.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(JobSeekerLoginDto dto)
        {
        var user = await _context.JobSeekers.FirstOrDefaultAsync(j => j.TcKimlikNo == dto.TcKimlikNo);
        if (user == null) return Unauthorized("Kullanıcı bulunamadı.");
    
        if (!VerifyPassword(dto.Password, user.PasswordHash))
        return Unauthorized("Parola hatalı.");

        var token = CreateToken(user);
        return Ok(new { token });
        }


        // parola hashleme
        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        // hashlenmiş parola kontrol etme
        private bool VerifyPassword(string password, string storedHash)
        {
            var hashOfInput = HashPassword(password);
            return hashOfInput == storedHash;
        }

        // JWT token oluşturma fonksiyonu
        private string CreateToken(JobSeeker user)
        {
            
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), 
                new Claim(ClaimTypes.Email, user.Email),                   
                new Claim(ClaimTypes.Role, "JobSeeker")                    
            };

            
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Token oluşturuluyor
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(1),   
                signingCredentials: creds
            );

           
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
