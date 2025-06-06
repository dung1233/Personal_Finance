namespace WebApplication4.Dtos
{
    public class AccountsSummaryResponseDto
    {
        public List<AccountBalanceDto> Accounts { get; set; } = new();
    }
}
