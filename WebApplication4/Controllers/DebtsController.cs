using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApplication4.Dtos;
using WebApplication4.Services;
using System.Threading.Tasks;

namespace WebApplication4.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DebtsController : ControllerBase
    {
        private readonly IDebtService _debtService;

        public DebtsController(IDebtService debtService)
        {
            _debtService = debtService;
        }

        [HttpGet]
        public async Task<ActionResult<List<DebtResponseDto>>> GetDebts()
        {
            int userId = GetUserId();
            var debts = await _debtService.GetDebtsAsync(userId);
            return Ok(debts);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DebtDetailResponseDto>> GetDebt(int id)
        {
            int userId = GetUserId();
            try
            {
                var debt = await _debtService.GetDebtByIdAsync(id, userId);
                return Ok(debt);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<DebtResponseDto>> CreateDebt([FromBody] CreateDebtRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            int userId = GetUserId();
            await _debtService.CreateDebtAsync(request, userId);

            var createdDebt = new DebtResponseDto
            {
                DebtName = request.DebtName,
                DebtType = request.DebtType,
                Creditor = request.Creditor,
                OriginalAmount = request.OriginalAmount,
                CurrentBalance = request.CurrentBalance,
                InterestRate = request.InterestRate,
                MinimumPayment = request.MinimumPayment,
                PaymentDueDate = request.PaymentDueDate,
                NextPaymentDate = request.NextPaymentDate,
                PayoffDate = request.PayoffDate,
                IsActive = true
            };

            return CreatedAtAction(nameof(GetDebt), new { id = 0 }, createdDebt);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDebt(int id, [FromBody] UpdateDebtRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            int userId = GetUserId();
            try
            {
                await _debtService.UpdateDebtAsync(id, request, userId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDebt(int id)
        {
            int userId = GetUserId();
            try
            {
                await _debtService.DeleteDebtAsync(id, userId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPatch("{id}/activate")]
        public async Task<IActionResult> ActivateDebt(int id, [FromBody] bool activate)
        {
            int userId = GetUserId();
            try
            {
                await _debtService.ActivateDebtAsync(id, activate, userId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("summary")]
        public async Task<ActionResult<DebtSummaryResponseDto>> GetDebtSummary()
        {
            int userId = GetUserId();
            var summary = await _debtService.GetDebtSummaryAsync(userId);
            return Ok(summary);
        }

        // Thêm endpoint mới để xử lý thanh toán nợ
        [HttpPost("{id}/process-payment")]
        public async Task<IActionResult> ProcessDebtPayment(int id, [FromBody] ProcessDebtPaymentRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            int userId = GetUserId();
            try
            {
                await _debtService.ProcessDebtPaymentAsync(id, request, userId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("due-notifications")]
        public async Task<ActionResult<IEnumerable<DebtDueNotificationDto>>> GetDebtDueNotifications([FromQuery] int days = 7)
        {
            int userId = GetUserId();
            var notifications = await _debtService.GetDebtDueNotificationsAsync(userId, days);
            return Ok(notifications);
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
                throw new UnauthorizedAccessException("Invalid user ID.");
            return userId;
        }
    }
}