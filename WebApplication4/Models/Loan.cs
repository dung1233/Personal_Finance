namespace WebApplication4.Models
{
    public class Loan
    {
        public int LoanId { get; set; }
        public int LenderId { get; set; }
        public string BorrowerName { get; set; }
        public string? BorrowerPhone { get; set; }
        public string? BorrowerEmail { get; set; }
        public string LoanName { get; set; }
        public string LoanType { get; set; }
        public decimal PrincipalAmount { get; set; }
        public decimal OutstandingBalance { get; set; }
        public decimal TotalReceived { get; set; }
        public decimal InterestRate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? NextPaymentDate { get; set; }
        public string PaymentFrequency { get; set; }
        public decimal? MinimumPayment { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public string? Notes { get; set; }
        public string? Contract { get; set; }
        public string? CollateralDescription { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime? LastPaymentDate { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ICollection<LoanPayment> Payments { get; set; }
    }

}
