using WebApplication4.Dtos;
using WebApplication4.Models;

namespace WebApplication4.Services
{
    public interface IInvestmentService
    {
        Task<List<InvestmentDto>> GetAllAsync();
        Task<InvestmentDto?> GetByIdAsync(int id);
        Task<InvestmentDto> CreateAsync(CreateInvestmentDto dto, int userId);
        Task<bool> UpdateAsync(int id, UpdateInvestmentDto dto);
        Task<bool> DeleteAsync(int id);
        Task<InvestmentSummaryDto> GetSummaryAsync();
    }
}