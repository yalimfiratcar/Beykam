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
    [Route("api/employer/auth")]  
    public class EmployerAuthController : ControllerBase
    {
        private readonly AppDbContext _context;   
        private readonly IConfiguration _config;  

        
        public EmployerAuthController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // İşveren kayıt olma 
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterEmployerDto dto)
        {
            // şifre eşleme test
            if (dto.Password != dto.ConfirmPassword)
                return BadRequest("Parolalar eşleşmiyor.");  

            // aynı email var mı test
            if (await _context.Employers.AnyAsync(e => e.Email == dto.Email))
                return BadRequest("Bu email zaten kayıtlı.");  

            // parola hashleme 
            var passwordHash = HashPassword(dto.Password);

            
            var employer = new Employer
            {
                TaxNumber = dto.TaxNumber,
                CompanyName = dto.CompanyName,
                TaxOffice = dto.TaxOffice,
                Email = dto.Email,
                PasswordHash = passwordHash
            };

            
            _context.Employers.Add(employer);
            await _context.SaveChangesAsync();

            
            return Ok("Kayıt başarılı.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(EmployerLoginDto dto)
        {
        var employer = await _context.Employers.FirstOrDefaultAsync(e => e.TaxNumber == dto.TaxNumber);
        if (employer == null) return Unauthorized("İşveren bulunamadı.");
 
        if (!VerifyPassword(dto.Password, employer.PasswordHash))
        return Unauthorized("Parola hatalı.");

        var token = CreateToken(employer);
        return Ok(new { token });
        }

 

        // parola hashlemee
        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        // parola kontrol için
        private bool VerifyPassword(string password, string storedHash)
        {
            var hashOfInput = HashPassword(password);
            return hashOfInput == storedHash;
        }

        // JWT token oluşturma fonksiyonu
        private string CreateToken(Employer user)
        {
            
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),  // Kullanıcı ID'si
                new Claim(ClaimTypes.Email, user.Email),                    // Email adresi
                new Claim(ClaimTypes.Role, "Employer")                      // Rol bilgisi (işveren)
            };

            
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Token nesnesi oluşturuluyo
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
