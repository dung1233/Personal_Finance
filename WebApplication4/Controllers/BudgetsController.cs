using Microsoft.AspNetCore.Mvc;
using WebApplication4.Dtos;
using WebApplication4.Services;
using System.Security.Claims;

namespace WebApplication4.Controllers
{
    [ApiController]
    [Route("api/budgets")]
    public class BudgetsController : ControllerBase
    {
        private readonly IBudgetService _budgetService;

        public BudgetsController(IBudgetService budgetService)
        {
            _budgetService = budgetService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBudgets()
        {
            var userId = GetUserIdFromClaims();
            if (userId == null) return Unauthorized("Invalid token or missing user ID");

            var budgets = await _budgetService.GetBudgetsAsync(userId.Value);
            return Ok(budgets);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBudget(int id)
        {
            var userId = GetUserIdFromClaims();
            if (userId == null) return Unauthorized("Invalid token or missing user ID");

            var budget = await _budgetService.GetBudgetByIdAsync(userId.Value, id);
            if (budget == null) return NotFound();
            return Ok(budget);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBudget([FromBody] CreateBudgetDto dto)
        {
            var userId = GetUserIdFromClaims();
            if (userId == null) return Unauthorized("Invalid token or missing user ID");

            var budget = await _budgetService.CreateBudgetAsync(userId.Value, dto);
            if (budget == null) return BadRequest("Invalid budget data or overlapping periods");
            return CreatedAtAction(nameof(GetBudget), new { id = budget.BudgetId }, budget);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBudget(int id, [FromBody] UpdateBudgetDto dto)
        {
            var userId = GetUserIdFromClaims();
            if (userId == null) return Unauthorized("Invalid token or missing user ID");

            var updated = await _budgetService.UpdateBudgetAsync(userId.Value, id, dto);
            if (updated == null) return BadRequest("Invalid data or overlapping budget period");
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBudget(int id)
        {
            var userId = GetUserIdFromClaims();
            if (userId == null) return Unauthorized("Invalid token or missing user ID");

            var result = await _budgetService.DeleteBudgetAsync(userId.Value, id);
            if (!result) return NotFound();
            return NoContent();
        }

        // Phương thức an toàn để lấy UserId từ Claims
        private int? GetUserIdFromClaims()
        {
            try
            {
                // Thử các claim type có thể có
                var userIdClaim = User.Claims.FirstOrDefault(c =>
                    c.Type == "nameid" ||
                    c.Type == ClaimTypes.NameIdentifier ||
                    c.Type == "sub" ||
                    c.Type == "user_id");

                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                {
                    return userId;
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        [HttpGet("current")]
        public async Task<IActionResult> GetCurrentBudgets()
        {
            var userId = GetUserIdFromClaims();
            if (userId == null) return Unauthorized("Invalid token or missing user ID");

            var budgets = await _budgetService.GetCurrentBudgetsAsync(userId.Value);
            return Ok(budgets);
        }
        [HttpGet("summary")]
        public async Task<IActionResult> GetBudgetSummary()
        {
            var userId = GetUserIdFromClaims();
            if (userId == null) return Unauthorized("Invalid token or missing user ID");
            var summary = await _budgetService.GetBudgetSummaryAsync(userId.Value);
            return Ok(summary);
        }
        [HttpGet("performance")]
        public async Task<IActionResult> GetBudgetPerformance()
        {
            var userId = GetUserIdFromClaims();
            if (userId == null) return Unauthorized("Invalid token or missing user ID");
            var performance = await _budgetService.GetBudgetPerformanceAsync(userId.Value);
            return Ok(performance);
        }
        [HttpPost("template")]
        public async Task<IActionResult> CreateBudgetsFromTemplate([FromBody] BudgetTemplateRequestDto dto)
        {
            var userId = GetUserIdFromClaims();
                if (userId == null) return Unauthorized("Invalid token or missing user ID");
            var budgets = await _budgetService.CreateBudgetsFromTemplateAsync(userId.Value, dto);
            return Ok(budgets);
        }
        [HttpPut("bulk")]
        public async Task<IActionResult> UpdateBudgetsBulk([FromBody] List<BudgetUpdateDto> budgets)
        {
            var userId = GetUserIdFromClaims();

            if (userId == null)
            {
                return Unauthorized("User not authenticated");
            }

            await _budgetService.UpdateBudgetsBulkAsync(userId.Value, budgets);
            return Ok("Cập nhật thành công"); // Fixed the issue by directly passing the string message.
        }
        [HttpGet("alerts")]
        public async Task<IActionResult> GetBudgetAlerts()
        {
            var userId = GetUserIdFromClaims();
            if (userId == null)
            {
                return Unauthorized("User not authenticated");
            }

            // DEBUG: Log userId
            Console.WriteLine($"UserId from claims: {userId.Value}");

            var result = await _budgetService.GetBudgetAlertsAsync(userId.Value);
            return Ok(result);
        }
        [HttpGet("categories/{categoryId}")]
        public async Task<IActionResult> GetBudgetsByCategory(int categoryId)
        {
            var userId = GetUserIdFromClaims();
            if (userId == null)
            {
                return Unauthorized("User not authenticated");
            }
            var budgets = await _budgetService.GetBudgetsByCategoryAsync(userId.Value, categoryId);
            return Ok(budgets);
        }
        [HttpGet("account-balance-alerts")]
        public async Task<IActionResult> GetAccountBalanceAlerts()
        {
            var userId = GetUserIdFromClaims();
            if (userId == null) return Unauthorized();

            var alerts = await _budgetService.GetAccountBalanceAlertsAsync(userId.Value);
            return Ok(alerts);
        }

        [HttpPost("check-feasibility")]
        public async Task<IActionResult> CheckBudgetFeasibility([FromBody] CreateBudgetDto dto)
        {
            var userId = GetUserIdFromClaims();
            if (userId == null) return Unauthorized();

            var result = await _budgetService.CheckBudgetFeasibilityAsync(userId.Value, dto);
            return Ok(result);
        }

        [HttpGet("financial-overview")]
        public async Task<IActionResult> GetFinancialOverview()
        {
            var userId = GetUserIdFromClaims();
            if (userId == null) return Unauthorized();

            var overview = await _budgetService.GetFinancialOverviewAsync(userId.Value);
            return Ok(overview);
        }




    }
}