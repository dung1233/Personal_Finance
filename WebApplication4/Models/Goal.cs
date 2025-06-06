using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication4.Models
{
    public class Goal
    {
        [Key]
        public int GoalId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required, MaxLength(100)]
        public string GoalName { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        [EnumDataType(typeof(GoalType))]
        public GoalType GoalType { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TargetAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal CurrentAmount { get; set; } = 0;

        public DateTime? TargetDate { get; set; }

        [Required]
        public GoalPriority Priority { get; set; } = GoalPriority.Medium;

        public bool IsCompleted { get; set; } = false;

        public DateTime? CompletedAt { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public User User { get; set; }
        public ICollection<GoalContribution> GoalContributions { get; set; } = new List<GoalContribution>();

    }
}
