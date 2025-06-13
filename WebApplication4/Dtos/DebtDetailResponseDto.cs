namespace WebApplication4.Dtos
{
    public class DebtDetailResponseDto : DebtResponseDto
    {
        public List<DebtPaymentDto> Payments { get; set; } = new();
    }

    public class DebtPaymentDto
    {
        public int PaymentId { get; set; }
        public decimal PaymentAmount { get; set; }
        public decimal? PrincipalAmount { get; set; }
        public decimal? InterestAmount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string? Notes { get; set; }
    }

}
