using WebApplication4.Dtos;

namespace WebApplication4.Services
{
    public interface IAccountService
    {
        Task<List<AccountDto>> GetAccountsAsync(int userId);
        Task<AccountDto?> GetAccountByIdAsync(int userId, int accountId);
        Task<AccountDto> CreateAccountAsync(int userId, CreateAccountDto dto);
        Task<bool> UpdateAccountAsync(int userId, int accountId, UpdateAccountDto dto);
        Task<bool> DeleteAccountAsync(int userId, int accountId);
        Task<bool> UpdateBalanceAsync(int userId, int accountId, AccountBalanceDto dto);
        Task<AccountSummaryDto> GetSummaryAsync(int userId);
        Task<List<string>> GetAccountTypesAsync();
    }
}
