using WebApplication4.Dtos;
using WebApplication4.Models;

namespace WebApplication4.Services
{
    public interface ILoanService
    {
        Task<LoanResponseDto> CreateLoanAsync(CreateLoanDto dto, int userId);
        Task<LoanPaymentResponseDto> CreateLoanPaymentAsync(CreateLoanPaymentDto dto, int userId);
        Task<List<LoanSummaryDto>> GetLoansAsync(int userId);
        Task<LoanResponseDto?> GetLoanByIdAsync(int loanId, int userId);
        Task<List<LoanPaymentDetailDto>> GetLoanPaymentsAsync(int loanId, int userId);
        // Thêm các method khác nếu cần
    }
}