using BCrypt.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using WebApplication4.Data;
using WebApplication4.Models;

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
            try
            {
                // Kiểm tra email đã tồn tại
                if (await _context.Users.AnyAsync(u => u.Email == request.Email))
                    return BadRequest("Email already exists.");

                // Kiểm tra role mặc định
                var defaultRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "User");
                if (defaultRole == null)
                    return StatusCode(500, "Default role not found");

                // Tạo user mới
                var user = new User
                {
                    Email = request.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    DateOfBirth = request.DateOfBirth,
                    PhoneNumber = request.PhoneNumber,
                    Currency = request.Currency ?? "USD", // Default currency
                    TimeZone = request.TimeZone ?? "UTC", // Default timezone
                    IsActive = true,
                    IsEmailVerified = false,
                    CreatedAt = DateTime.UtcNow,
                    RoleId = defaultRole.RoleId
                };

                // Tạo activation code
                var activationCode = Guid.NewGuid().ToString();
                user.EmailActivationCode = activationCode;
                user.EmailActivationCodeExpires = DateTime.UtcNow.AddMinutes(15);

                // Lưu vào database
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Gửi email kích hoạt
                try
                {
                    await SendActivationEmail(user.Email, activationCode);
                    return Ok(new
                    {
                        message = "User registered successfully. Please check your email to activate your account.",
                        userId = user.UserId
                    });
                }
                catch (Exception emailEx)
                {
                    // Nếu gửi email thất bại, vẫn trả về success nhưng thông báo
                    return Ok(new
                    {
                        message = "User registered successfully, but activation email could not be sent. Please contact support.",
                        userId = user.UserId,
                        emailError = true
                    });
                }
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"Database error: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
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

            // ✅ Kiểm tra tài khoản đã xác minh email chưa
            if (!user.IsEmailVerified)
            {
                return Unauthorized("Your account has not been activated. Please check your email for the activation link.");
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

        private async Task SendActivationEmail(string toEmail, string activationCode)
        {
            try
            {
                var emailSettings = _configuration.GetSection("EmailSettings");
                var baseUrl = _configuration.GetValue<string>("AppSettings:BaseUrl");

                var fromEmail = emailSettings["FromEmail"];
                var fromPassword = emailSettings["FromPassword"];
                var fromName = emailSettings["FromName"];
                var smtpServer = emailSettings["SmtpServer"];
                var smtpPort = emailSettings.GetValue<int>("SmtpPort");

                var activationLink = $"{baseUrl}/api/auth/activate?code={activationCode}";

                var mail = new MailMessage();
                mail.From = new MailAddress(fromEmail, fromName);
                mail.To.Add(toEmail);
                mail.Subject = "Kích hoạt tài khoản - Your App Name";

                // HTML email template
                mail.Body = $@"
            <html>
            <body>
                <h2>Chào mừng bạn đến với Personalfinance!</h2>
                <p>Cảm ơn bạn đã đăng ký tài khoản. Để hoàn tất quá trình đăng ký, vui lòng nhấn vào nút bên dưới:</p>
                <p style='text-align: center; margin: 30px 0;'>
                    <a href='{activationLink}' 
                       style='background-color: #007bff; color: white; padding: 12px 25px; 
                              text-decoration: none; border-radius: 5px; display: inline-block;'>
                        Kích hoạt tài khoản
                    </a>
                </p>
                <p>Hoặc copy link sau vào trình duyệt:</p>
                <p><a href='{activationLink}'>{activationLink}</a></p>
                <p><small>Link này sẽ hết hạn sau 15 phút.</small></p>
                <hr>
                <p><small>Nếu bạn không đăng ký tài khoản này, vui lòng bỏ qua email này.</small></p>
            </body>
            </html>";

                mail.IsBodyHtml = true;

                using var smtp = new SmtpClient(smtpServer, smtpPort)
                {
                    Credentials = new NetworkCredential(fromEmail, fromPassword),
                    EnableSsl = true
                };

                await smtp.SendMailAsync(mail);
            }
            catch (Exception ex)
            {
                // Log error (nên dùng ILogger)
                Console.WriteLine($"Error sending activation email: {ex.Message}");
                // Có thể throw exception hoặc handle theo business logic
                throw new Exception("Không thể gửi email kích hoạt. Vui lòng thử lại sau.");
            }
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

        [Authorize(Roles = "Admin")]
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _context.Users
                .Include(u => u.Role)
                .Select(u => new
                {
                    u.UserId,
                    u.Email,
                    u.FirstName,
                    u.LastName,
                    u.DateOfBirth,
                    u.PhoneNumber,
                    u.IsActive,
                    u.IsEmailVerified,
                    u.CreatedAt,
                    u.LastLoginAt,
                    Role = u.Role != null ? u.Role.Name : null
                })
                .ToListAsync();

            return Ok(users);
        }

        [HttpGet("activate")]
        public async Task<IActionResult> Activate([FromQuery] string code)
        {
            try
            {
                if (string.IsNullOrEmpty(code))
                    return BadRequest("Activation code is required.");

                var user = await _context.Users.FirstOrDefaultAsync(u =>
                    u.EmailActivationCode == code &&
                    u.EmailActivationCodeExpires > DateTime.UtcNow);

                if (user == null)
                {
                    // Kiểm tra xem user đã được kích hoạt chưa
                    var activatedUser = await _context.Users.FirstOrDefaultAsync(u =>
                        u.EmailActivationCode == code && u.IsEmailVerified);

                    if (activatedUser != null)
                        return BadRequest("Account has already been activated.");

                    return BadRequest("Invalid or expired activation code.");
                }

                // Kích hoạt tài khoản
                user.IsEmailVerified = true;
                user.EmailActivationCode = null;
                user.EmailActivationCodeExpires = null;
                user.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                // Có thể redirect về trang login hoặc trả về success page
                return Ok(new
                {
                    message = "Account activated successfully. You can now login.",
                    email = user.Email
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred during activation: {ex.Message}");
            }
        }

        // Thêm endpoint để gửi lại email kích hoạt
        [HttpPost("resend-activation")]
        public async Task<IActionResult> ResendActivation([FromBody] ResendActivationRequest request)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

                if (user == null)
                    return NotFound("User not found.");

                if (user.IsEmailVerified)
                    return BadRequest("Account is already activated.");

                // Tạo mã kích hoạt mới
                var activationCode = Guid.NewGuid().ToString();
                user.EmailActivationCode = activationCode;
                user.EmailActivationCodeExpires = DateTime.UtcNow.AddMinutes(15);
                user.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                await SendActivationEmail(user.Email, activationCode);

                return Ok("Activation email has been resent.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        public class ResendActivationRequest
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; } = null!;
        }
    }
}
