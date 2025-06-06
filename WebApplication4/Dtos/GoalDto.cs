using System;
using WebApplication4.Models;

namespace WebApplication4.Dtos
{
    public class GoalDto
    {
        public int GoalId { get; set; }

        public string GoalName { get; set; }

        public string? Description { get; set; }

        public GoalType GoalType { get; set; }

        public decimal TargetAmount { get; set; }

        public decimal CurrentAmount { get; set; }

        public DateTime? TargetDate { get; set; }

        public GoalPriority Priority { get; set; }

        public bool IsCompleted { get; set; }

        public DateTime? CompletedAt { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
