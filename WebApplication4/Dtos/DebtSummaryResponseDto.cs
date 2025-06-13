namespace WebApplication4.Dtos
{
    public class DebtSummaryResponseDto
    {
        public int TotalDebts { get; set; }
        public decimal TotalOriginalAmount { get; set; }
        public decimal TotalCurrentBalance { get; set; }
        public decimal TotalMonthlyPayment { get; set; }
    }

}
