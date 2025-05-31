using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApplication4.Dtos;
using WebApplication4.Dtos.Transactions;
using WebApplication4.Services;
namespace WebApplication4.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TransactionsController : ControllerBase
    {
        private readonly TransactionService _transactionService;

        public TransactionsController(TransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        private int GetUserId()
        {
            var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (id == null) throw new UnauthorizedAccessException("User ID missing");
            return int.Parse(id);
        }

        [HttpGet]
        public async Task<IActionResult> GetTransactions()
        {
            var userId = GetUserId();
            var transactions = await _transactionService.GetTransactionsAsync(userId);
            return Ok(transactions);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTransactionById(int id)
        {
            var userId = GetUserId();
            var transaction = await _transactionService.GetTransactionByIdAsync(userId, id);
            if (transaction == null) return NotFound();
            return Ok(transaction);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTransaction(CreateTransactionDto dto)
        {
            var userId = GetUserId();
            var success = await _transactionService.CreateTransactionAsync(userId, dto);
            if (!success) return BadRequest("Invalid account or data.");
            return Ok(new { message = "Transaction created." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransaction(int id)
        {
            var userId = GetUserId();
            var success = await _transactionService.DeleteTransactionAsync(userId, id);
            if (!success) return NotFound();
            return Ok(new { message = "Transaction deleted." });
        }
        [HttpGet("search")]
        public async Task<IActionResult> SearchTransactions([FromQuery] string? keyword)
        {
            var userId = GetUserId();
            var results = await _transactionService.SearchTransactionsAsync(userId, keyword);
            return Ok(results);
        }
        [HttpGet("recent")]
        public async Task<IActionResult> GetRecentTransactions()
        {
            var userId = GetUserId();
            var results = await _transactionService.GetRecentTransactionsAsync(userId);
            return Ok(results);
        }
        [HttpPost("bulk")]
        public async Task<IActionResult> ImportBulkTransactions(BulkTransactionDto dto)
        {
            var userId = GetUserId();
            var count = await _transactionService.ImportBulkTransactionsAsync(userId, dto.Transactions);
            return Ok(new { imported = count });
        }
        [HttpGet("recurring")]
        public async Task<IActionResult> GetRecurringTransactions()
        {
            var userId = GetUserId();
            var recurringTransactions = await _transactionService.GetRecurringTransactionsAsync(userId);
            return Ok(recurringTransactions);
        }
        [HttpPut("{id}/category")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] UpdateCategoryDto dto)
        {
            var userId = GetUserId();
            var success = await _transactionService.UpdateTransactionCategoryAsync(userId, id, dto.CategoryId);
            if (!success) return NotFound();
            return Ok(new { message = "Category updated." });
        }
        [HttpPost("transfer")]
        public async Task<IActionResult> Transfer([FromBody] TransferDto dto)
        {
            var userId = GetUserId();
            var success = await _transactionService.TransferAsync(userId, dto);
            if (!success) return BadRequest("Invalid transfer.");
            return Ok(new { message = "Transfer successful." });
        }
        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            var userId = GetUserId();
            var summary = await _transactionService.GetTransactionSummaryAsync(userId, from, to);
            return Ok(summary);
        }








    }
}
