namespace WebApplication4.Dtos
{
    public class LoanResponseDto
    {
        public int LoanId { get; set; }
        public string BorrowerName { get; set; }
        public string LoanName { get; set; }
        public decimal PrincipalAmount { get; set; }
        public decimal OutstandingBalance { get; set; }
        public decimal TotalReceived { get; set; }
        public decimal InterestRate { get; set; }
        public string Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? DueDate { get; set; }
    }

}
