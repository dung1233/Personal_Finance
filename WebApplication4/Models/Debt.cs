using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApplication4.Models
{
    public enum DebtType
    {
        CreditCard,
        StudentLoan,
        Mortgage,
        PersonalLoan,
        AutoLoan,
        Other
    }

    public class Debt
    {
        [Key]
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
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public List<DebtPayment> Payments { get; set; } = new();
    }
}
