using System;
using System.Collections.Generic;

namespace WebApplication4.Dtos
{
    public class DebtDetailResponseDto : DebtResponseDto
    {
        public List<DebtPaymentDto> PaymentHistory { get; set; } = new List<DebtPaymentDto>();
    }

    public class DebtPaymentDto
    {
        public int PaymentId { get; set; }
        public decimal PaymentAmount { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal? PrincipalAmount { get; set; }
        public decimal? InterestAmount { get; set; }
        public string? Notes { get; set; }
    }
}
