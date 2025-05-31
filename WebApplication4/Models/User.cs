using System.ComponentModel.DataAnnotations;
namespace WebApplication4.Models
{
    public class User
    {
        [Key]
        public int? UserId { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
        [Required]
        public string PasswordHash { get; set; } = null!;
        [MaxLength(255)]
        public string? FirstName { get; set; }
        [MaxLength(255)]
        public string? LastName { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [MaxLength(15)]
        public string? PhoneNumber { get; set; }
        [MaxLength(10)]
        public string? Currency {  get; set; }

        [MaxLength(50)]
        public string? TimeZone { get; set; }

        public bool IsActive { get; set; } = true;

        public bool IsEmailVerified { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public DateTime? LastLoginAt { get; set; }

        public int? RoleId { get; set; }
        public Role? Role { get; set; }


    }
}
