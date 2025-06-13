namespace WebApplication4.Dtos
{
    public class LoanAlertDto
    {
        public int LoanId { get; set; }
        public string LoanName { get; set; }
        public string BorrowerName { get; set; }
        public DateTime DueDate { get; set; }
        public decimal OutstandingBalance { get; set; }
        public string Status { get; set; }
        public int DaysUntilDue { get; set; }  // âm là quá hạn, dương là còn bao nhiêu ngày
    }

}
