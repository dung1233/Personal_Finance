namespace WebApplication4.Dtos
{
    public class ContributionUpdateRequest
    {
        public decimal Amount { get; set; }
        public string? Note { get; set; }
        public DateTime Date { get; set; }
    }

}
