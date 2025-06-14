using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication4.Models
{



    public class Debt
    {
        public int DebtId { get; set; }
        public int UserId { get; set; }
        public string DebtName { get; set; } = null!;
        public string DebtType { get; set; } = null!;
        public string? Creditor { get; set; }
        public decimal OriginalAmount { get; set; }
        public decimal CurrentBalance { get; set; }
        public decimal? InterestRate { get; set; }
        public decimal? MinimumPayment { get; set; }
        public int? PaymentDueDate { get; set; }
        public DateTime? NextPaymentDate { get; set; }
        public DateTime? PayoffDate { get; set; }
        public int? AccountId { get; set; } // Thêm AccountId
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public User User { get; set; } = null!;
        public Account? Account { get; set; } // Navigation property
        public ICollection<DebtPayment> DebtPayments { get; set; } = new List<DebtPayment>();
    }
}
