namespace WebApplication4.Dtos
{
    public class ContributionDto
    {
        public decimal Amount { get; set; }
        public string? Note { get; set; }
        public DateTime ContributionDate { get; set; } // Added missing property
    }
}
