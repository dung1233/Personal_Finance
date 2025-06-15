using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication4.Models
{
    [Table("loanpayments")]
    public class LoanPayment
    {
        [Key]
        public int PaymentId { get; set; }

        [Required]
        public int LoanId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PaymentAmount { get; set; } // Số tiền thu được

        [Required]
        [Column(TypeName = "date")]
        public DateTime PaymentDate { get; set; } // Ngày thu tiền

        [Column(TypeName = "decimal(18,2)")]
        public decimal? PrincipalAmount { get; set; } // Tiền gốc

        [Column(TypeName = "decimal(18,2)")]
        public decimal? InterestAmount { get; set; } // Tiền lãi

        [StringLength(50)]
        public string PaymentMethod { get; set; } = "Cash"; // Phương thức thanh toán

        [StringLength(500)]
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("LoanId")]
        public virtual Loan Loan { get; set; } = null!;

        // Computed properties
        [NotMapped]
        public bool IsPartialPayment => Loan != null && PaymentAmount < (Loan.ExpectedPayment ?? Loan.CurrentBalance);

        [NotMapped]
        public bool IsOverPayment => Loan != null && PaymentAmount > Loan.CurrentBalance;

        [NotMapped]
        public string PaymentStatus
        {
            get
            {
                if (Loan == null) return "Unknown";

                if (IsOverPayment) return "Overpaid";
                if (IsPartialPayment) return "Partial";
                return "Full";
            }
        }
    }

    // Enum cho các phương thức thanh toán
    public enum PaymentMethod
    {
        Cash,
        BankTransfer,
        CreditCard,
        DebitCard,
        MobilePay,
        Check,
        Other
    }
}