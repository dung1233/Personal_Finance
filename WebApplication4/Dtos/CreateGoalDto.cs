using System;
using System.ComponentModel.DataAnnotations;
using WebApplication4.Models;

namespace WebApplication4.Dtos
{
    public class CreateGoalDto
    {
        [Required, MaxLength(100)]
        public string GoalName { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        public GoalType GoalType { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal TargetAmount { get; set; }

        public DateTime? TargetDate { get; set; }

        public GoalPriority Priority { get; set; } = GoalPriority.Medium;
    }
}
