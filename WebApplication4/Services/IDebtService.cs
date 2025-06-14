using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication4.Dtos;

namespace WebApplication4.Services
{
    public interface IDebtService
    {
        Task<List<DebtResponseDto>> GetDebtsAsync(int userId);
        Task<DebtDetailResponseDto> GetDebtByIdAsync(int debtId, int userId);
        Task CreateDebtAsync(CreateDebtRequestDto request, int userId);
        Task UpdateDebtAsync(int debtId, UpdateDebtRequestDto request, int userId);
        Task DeleteDebtAsync(int debtId, int userId);
        Task ActivateDebtAsync(int debtId, bool activate, int userId);
        Task<DebtSummaryResponseDto> GetDebtSummaryAsync(int userId);
        Task ProcessDebtPaymentAsync(int debtId, ProcessDebtPaymentRequestDto request, int userId);
    }
}
