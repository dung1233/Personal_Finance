using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication4.Models
{
    public class Budget
    {
        [Key]
        public int BudgetId { get; set; }

        public int UserId { get; set; }
        public int CategoryId { get; set; }

        [Required, StringLength(100)]
        public string BudgetName { get; set; } = null!;

        [Column(TypeName = "decimal(18,2)")]
        public decimal BudgetAmount { get; set; }

        [Required]
        [StringLength(20)]
        public string BudgetPeriod { get; set; } = null!; // Weekly, Monthly, Quarterly, Yearly

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal SpentAmount { get; set; } = 0;

        [Column(TypeName = "decimal(5,2)")]
        public decimal AlertThreshold { get; set; } = 80m;

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public User? User { get; set; }
        public Category? Category { get; set; }
    }
}
