using WebApplication4.Dtos;

namespace WebApplication4.Services
{
    public interface IDashboardService
    {
        Task<DashboardOverviewResponseDto> GetOverviewAsync(int userId);
       
        Task<AccountsSummaryResponseDto> GetAccountsSummaryAsync(int userId);
    }

}
