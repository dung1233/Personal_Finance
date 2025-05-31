namespace WebApplication4.Models
{
    public class Transaction
    {
        public int TransactionId { get; set; }
        public int UserId { get; set; }
        public int AccountId { get; set; }
        public int? CategoryId { get; set; }

        public decimal Amount { get; set; }
        public string TransactionType { get; set; } = null!;  // "Income", "Expense", "Transfer"
        public string? Description { get; set; }
        public DateTime TransactionDate { get; set; }

        public string? Merchant { get; set; }
        public string? Notes { get; set; }
        public string? Tags { get; set; }

        public bool IsRecurring { get; set; } = false;
        public string? RecurringFrequency { get; set; }      // "Daily", "Weekly", etc.
        public DateTime? NextRecurringDate { get; set; }

        public string? Receipt { get; set; }   // URL hoặc path

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public User? User { get; set; }
        public Account? Account { get; set; }
        public Category? Category { get; set; }
    }
}
