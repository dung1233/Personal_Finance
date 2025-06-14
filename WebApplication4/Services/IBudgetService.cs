using WebApplication4.Dtos;

namespace WebApplication4.Services
{
    public interface IBudgetService
    {
        Task<BudgetDto?> CreateBudgetAsync(int userId, CreateBudgetDto dto);
        Task<IEnumerable<BudgetDto>> GetBudgetsAsync(int userId);
        Task<BudgetDto?> GetBudgetByIdAsync(int userId, int budgetId);
        Task<BudgetDto?> UpdateBudgetAsync(int userId, int budgetId, UpdateBudgetDto dto);
        Task<bool> DeleteBudgetAsync(int userId, int budgetId);
        Task<List<BudgetDto>> GetCurrentBudgetsAsync(int userId);
        Task<List<BudgetSummaryDto>> GetBudgetSummaryAsync(int userId);
        Task<List<BudgetPerformanceDto>> GetBudgetPerformanceAsync(int userId);
        Task<List<BudgetDto>> CreateBudgetsFromTemplateAsync(int userId, BudgetTemplateRequestDto dto);
        Task<List<BudgetDto>> UpdateBudgetsBulkAsync(int userId, List<BudgetUpdateDto> budgets);
        Task<List<BudgetAlertDto>> GetBudgetAlertsAsync(int userId);
       
        Task<List<BudgetDto>> GetBudgetsByCategoryAsync(int userId, int categoryId);
        Task<List<AccountBalanceAlertDto>> GetAccountBalanceAlertsAsync(int userId);
        Task<BudgetFeasibilityCheckDto> CheckBudgetFeasibilityAsync(int userId, CreateBudgetDto dto);
        Task<FinancialOverviewDto> GetFinancialOverviewAsync(int userId);

    }

}
