using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApplication4.Dtos;
using WebApplication4.Services;

namespace WebApplication4.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Chỉ cho phép người dùng đã đăng nhập
    public class AccountsController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountsController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        // Giả sử ta có UserId từ token (giản lược để demo)
        private int GetUserId() =>
    int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
               ?? throw new UnauthorizedAccessException("User ID claim missing"));


        [HttpGet]
        public async Task<IActionResult> GetAccounts()
        {
            var accounts = await _accountService.GetAccountsAsync(GetUserId());
            return Ok(accounts);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAccountById(int id)
        {
            var account = await _accountService.GetAccountByIdAsync(GetUserId(), id);
            return account == null ? NotFound() : Ok(account);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAccount([FromBody] CreateAccountDto dto)
        {
            try
            {
                var account = await _accountService.CreateAccountAsync(GetUserId(), dto);
                return CreatedAtAction(nameof(GetAccountById), new { id = account.AccountId }, account);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAccount(int id, [FromBody] UpdateAccountDto dto)
        {
            var result = await _accountService.UpdateAccountAsync(GetUserId(), id, dto);
            return result ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            var result = await _accountService.DeleteAccountAsync(GetUserId(), id);
            return result ? NoContent() : NotFound();
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary()
        {
            var summary = await _accountService.GetSummaryAsync(GetUserId());
            return Ok(summary);
        }

        [HttpPut("{id}/balance")]
        public async Task<IActionResult> UpdateBalance(int id, [FromBody] AccountBalanceDto dto)
        {
            var result = await _accountService.UpdateBalanceAsync(GetUserId(), id, dto);
            return result ? NoContent() : NotFound();
        }

        [HttpGet("types")]
        public async Task<IActionResult> GetAccountTypes()
        {
            var types = await _accountService.GetAccountTypesAsync();
            return Ok(types);
        }
    }
}
