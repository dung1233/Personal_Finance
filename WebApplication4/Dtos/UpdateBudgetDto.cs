using System.ComponentModel.DataAnnotations;

namespace WebApplication4.Dtos
{
    public class UpdateBudgetDto
    {
        [Required]
        public string BudgetName { get; set; } = null!;

        [Range(0.01, double.MaxValue)]
        public decimal BudgetAmount { get; set; }

        [RegularExpression("Weekly|Monthly|Quarterly|Yearly")]
        public string BudgetPeriod { get; set; } = null!;

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public decimal AlertThreshold { get; set; } = 80m;

        public bool IsActive { get; set; }

        public int CategoryId { get; set; }
    }

}
