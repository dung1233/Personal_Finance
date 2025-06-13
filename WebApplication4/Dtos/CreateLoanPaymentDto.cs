namespace WebApplication4.Dtos
{
    public class CreateLoanPaymentDto
    {
        public decimal PaymentAmount { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal? PrincipalAmount { get; set; }
        public decimal? InterestAmount { get; set; }
        public decimal? LateFee { get; set; }
        public string PaymentMethod { get; set; }
        public string? Notes { get; set; }
    }

}
