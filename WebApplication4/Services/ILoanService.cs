using WebApplication4.Dtos;

namespace WebApplication4.Services
{
    public interface ILoanService
    {
        Task<List<LoanResponseDto>> GetLoansAsync(int userId);
        Task<LoanResponseDto> GetLoanByIdAsync(int loanId, int userId);
        Task<int> CreateLoanAsync(int userId, CreateLoanRequestDto dto);
        Task AddPaymentAsync(int loanId, CreateLoanPaymentDto dto, int userId);
        Task<LoanSummaryDto> GetLoanSummaryAsync(int userId);
        Task<List<LoanAlertDto>> GetLoanAlertsAsync(int userId);
    }

}
