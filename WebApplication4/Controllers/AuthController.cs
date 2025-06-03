using BCrypt.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApplication4.Data;
using WebApplication4.Models;
using Microsoft.AspNetCore.Authorization;
namespace WebApplication4.Controllers
{
   
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
                return BadRequest("Email already exists.");
            var defaultRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "User");
            if (defaultRole == null)
                return StatusCode(500, "Default role not found");
            var user = new User
            {
                // KHÔNG set UserId tại đây, EF sẽ tự tạo
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                FirstName = request.FirstName,
                LastName = request.LastName,
                DateOfBirth = request.DateOfBirth,
                PhoneNumber = request.PhoneNumber,
                Currency = request.Currency,
                TimeZone = request.TimeZone,
                IsActive = true,
                IsEmailVerified = false,
                CreatedAt = DateTime.UtcNow,
                RoleId = defaultRole.RoleId
            };

            _context.Users.Add(user);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return StatusCode(500, $"Concurrency error: {ex.Message}");
            }

            return Ok("User registered successfully.");
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _context.Users.Include(u => u.Role)
                          .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return Unauthorized("Invalid email or password");
            }

            user.LastLoginAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            var token = GenerateJwtToken(user);

            return Ok(new { token });
        }

        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}")
    };

            if (user.Role != null)
            {
                claims.Add(new Claim(ClaimTypes.Role, user.Role.Name));
            }

            // Cách 1: Sử dụng GetValue với default value
            var expireMinutes = _configuration.GetValue<double>("Jwt:ExpireMinutes", 60);

            // Hoặc Cách 2: Kiểm tra null trước khi parse
            // var expireMinutesString = jwtSettings["ExpireMinutes"];
            // var expireMinutes = string.IsNullOrEmpty(expireMinutesString) ? 60 : double.Parse(expireMinutesString);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(expireMinutes),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }


        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetProfile()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();

            var userId = userIdClaim.Value;
            var user = await _context.Users.FindAsync(Guid.Parse(userId));
            if (user == null) return NotFound();

            return Ok(new
            {
                user.UserId,
                user.Email,
                user.FirstName,
                user.LastName,
                user.DateOfBirth,
                user.PhoneNumber
            });
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("admin-only")]
        public IActionResult GetAdminData()
        {
            return Ok("Hello Admin");
        }
    }
}
