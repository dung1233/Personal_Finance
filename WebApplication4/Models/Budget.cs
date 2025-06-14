using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication4.Models
{
    public class Budget
    {
        public int BudgetId { get; set; }
        public int UserId { get; set; }
        public int CategoryId { get; set; }
        public string BudgetName { get; set; }
        public decimal BudgetAmount { get; set; }
        public string BudgetPeriod { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal SpentAmount { get; set; }
        public decimal AlertThreshold { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public User? User { get; set; }
        public Category? Category { get; set; }

        // Add the missing AccountId property
        public int AccountId { get; set; }
    }
}
