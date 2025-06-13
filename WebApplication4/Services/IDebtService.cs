using WebApplication4.Dtos;

namespace WebApplication4.Services
{
    public interface IDebtService
    {
        Task<List<DebtResponseDto>> GetAllDebtsAsync(int userId);
        Task<DebtDetailResponseDto?> GetDebtByIdAsync(int userId, int debtId);
        Task<DebtResponseDto> CreateDebtAsync(int userId, CreateDebtRequestDto dto);
        Task<DebtResponseDto?> UpdateDebtAsync(int userId, int debtId, UpdateDebtRequestDto dto);
        Task<bool> DeleteDebtAsync(int userId, int debtId);
        Task<bool> ToggleDebtActiveStatusAsync(int userId, int debtId);
        Task<DebtSummaryResponseDto> GetDebtSummaryAsync(int userId);
    }

}
