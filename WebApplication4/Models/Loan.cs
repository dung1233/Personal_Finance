using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication4.Models
{
    [Table("loans")]
    public class Loan
    {
        [Key]
        public int LoanId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string LoanName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string LoanType { get; set; } = string.Empty; // Personal Loan, Business Loan, Emergency Loan, Family Loan, Friend Loan, Other

        [Required]
        [StringLength(100)]
        public string Borrower { get; set; } = string.Empty; // Tên người vay

        [StringLength(20)]
        public string? BorrowerPhone { get; set; }

        [StringLength(255)]
        [EmailAddress]
        public string? BorrowerEmail { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal OriginalAmount { get; set; } // Số tiền gốc cho vay

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal CurrentBalance { get; set; } // Số tiền còn lại phải thu

        [Column(TypeName = "decimal(5,2)")]
        public decimal InterestRate { get; set; } = 0.00m; // Lãi suất (% năm)

        [Column(TypeName = "decimal(18,2)")]
        public decimal? ExpectedPayment { get; set; } // Số tiền dự kiến thu mỗi kỳ

        public int? PaymentDueDate { get; set; } // Ngày đáo hạn trong tháng (1-31)

        [Column(TypeName = "date")]
        public DateTime? NextPaymentDate { get; set; } // Ngày thu tiền tiếp theo

        [Required]
        [Column(TypeName = "date")]
        public DateTime LoanDate { get; set; } // Ngày cho vay

        [Column(TypeName = "date")]
        public DateTime? DueDate { get; set; } // Ngày đáo hạn toàn bộ khoản vay

        [StringLength(500)]
        public string? ContractDocument { get; set; } // Đường dẫn file hợp đồng

        [Column(TypeName = "text")]
        public string? Notes { get; set; } // Ghi chú

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public int AccountId { get; set; } // Tài khoản đã cho vay (bắt buộc)

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;

        [ForeignKey("AccountId")]
        public virtual Account Account { get; set; } = null!;

        public virtual ICollection<LoanPayment> LoanPayments { get; set; } = new List<LoanPayment>();

        // Computed properties
        [NotMapped]
        public decimal TotalPaid => LoanPayments?.Sum(p => p.PaymentAmount) ?? 0;

        [NotMapped]
        public decimal RemainingAmount => OriginalAmount - TotalPaid;

        [NotMapped]
        public bool IsOverdue => NextPaymentDate.HasValue && NextPaymentDate.Value < DateTime.Today && CurrentBalance > 0;

        [NotMapped]
        public int DaysOverdue => IsOverdue ? (DateTime.Today - NextPaymentDate!.Value).Days : 0;

        [NotMapped]
        public bool IsFullyPaid => CurrentBalance <= 0;

        [NotMapped]
        public decimal CompletionPercentage => OriginalAmount > 0 ? (TotalPaid / OriginalAmount) * 100 : 0;
    }

    // Enum cho các loại khoản vay
    public enum LoanType
    {
        PersonalLoan,
        BusinessLoan,
        EmergencyLoan,
        FamilyLoan,
        FriendLoan,
        Other
    }
}