using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApplication4.Dtos;
using WebApplication4.Models;
using WebApplication4.Services;

namespace WebApplication4.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DebtController : ControllerBase
    {
        private readonly IDebtService _debtService;

        public DebtController(IDebtService debtService)
        {
            _debtService = debtService;
        }

        private int GetUserId() => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

        [HttpGet]
        public async Task<IActionResult> GetAllDebts()
        {
            var userId = GetUserId();
            var debts = await _debtService.GetAllDebtsAsync(userId);
            return Ok(debts);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDebtById(int id)
        {
            var userId = GetUserId();
            var debt = await _debtService.GetDebtByIdAsync(userId, id);
            if (debt == null) return NotFound();
            return Ok(debt);
        }

        [HttpPost]
        public async Task<IActionResult> CreateDebt([FromBody] CreateDebtRequestDto dto)
        {
            var userId = GetUserId();
            var result = await _debtService.CreateDebtAsync(userId, dto);
            return CreatedAtAction(nameof(GetDebtById), new { id = result.DebtId }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDebt(int id, [FromBody] UpdateDebtRequestDto dto)
        {
            var userId = GetUserId();
            var result = await _debtService.UpdateDebtAsync(userId, id, dto);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDebt(int id)
        {
            var userId = GetUserId();
            var deleted = await _debtService.DeleteDebtAsync(userId, id);
            if (!deleted) return NotFound();
            return NoContent();
        }

        [HttpPatch("{id}/activate")]
        public async Task<IActionResult> ToggleActiveStatus(int id)
        {
            var userId = GetUserId();
            var toggled = await _debtService.ToggleDebtActiveStatusAsync(userId, id);
            if (!toggled) return NotFound();
            return Ok();
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetDebtSummary()
        {
            var userId = GetUserId();
            var summary = await _debtService.GetDebtSummaryAsync(userId);
            return Ok(summary);
        }
    }
}