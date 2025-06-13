namespace WebApplication4.Dtos
{
    public class LoanSummaryDto
    {
        public int TotalLoans { get; set; }
        public decimal TotalLent { get; set; }
        public decimal TotalReceived { get; set; }
        public decimal TotalOutstanding { get; set; }
        public decimal ActiveOutstanding { get; set; }
        public decimal OverdueAmount { get; set; }
        public decimal AvgInterestRate { get; set; }
        public decimal TotalInterestEarned { get; set; }
    }

}
