using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApplication4.Dtos;
using WebApplication4.Services;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LoansController : ControllerBase
{
    private readonly ILoanService _loansService;

    public LoansController(ILoanService loansService)
    {
        _loansService = loansService;
    }

    private int GetUserId()
    {
        // Thử cả 2 loại claim
        var claim = User.FindFirst("nameid") ?? User.FindFirst(ClaimTypes.NameIdentifier);

        if (claim == null)
            throw new UnauthorizedAccessException("User ID not found in token.");

        return int.Parse(claim.Value);
    }


    // GET: /api/loans
    [HttpGet]
    public async Task<ActionResult<List<LoanResponseDto>>> GetLoans()
    {
        int userId = GetUserId();
        var loans = await _loansService.GetLoansAsync(userId);
        return Ok(loans);
    }

    // GET: /api/loans/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<LoanResponseDto>> GetLoanById(int id)
    {
        try
        {
            int userId = GetUserId();
            var loan = await _loansService.GetLoanByIdAsync(id, userId); // ✅ Fixed parameter order
            return Ok(loan);
        }
        catch (Exception)
        {
            return NotFound();
        }
    }

    // POST: /api/loans
    [HttpPost]
    public async Task<ActionResult> CreateLoan([FromBody] CreateLoanRequestDto dto)
    {
        int userId = GetUserId();
        var loanId = await _loansService.CreateLoanAsync(userId, dto); // ✅ Fixed return type
        return CreatedAtAction(nameof(GetLoanById), new { id = loanId }, new { LoanId = loanId });
    }

    // POST: /api/loans/{id}/payments
    [HttpPost("{id}/payments")]
    public async Task<IActionResult> AddPayment(int id, [FromBody] CreateLoanPaymentDto dto)
    {
        try
        {
            int userId = GetUserId();
            await _loansService.AddPaymentAsync(id, dto, userId);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                message = ex.InnerException?.Message ?? ex.Message
            });
        }

    }


    // GET: /api/loans/summary
    [HttpGet("summary")]
    public async Task<ActionResult<LoanSummaryDto>> GetSummary()
    {
        int userId = GetUserId();
        var summary = await _loansService.GetLoanSummaryAsync(userId);
        return Ok(summary);
    }

    [HttpGet("alerts")]
    public async Task<IActionResult> GetLoanAlerts()
    {
        var userId = GetUserId();
        var alerts = await _loansService.GetLoanAlertsAsync(userId);
        return Ok(alerts);
    }

}