using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApplication4.Dtos;
using WebApplication4.Services;

namespace WebApplication4.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class LoansController : ControllerBase
    {
        private readonly ILoanService _loanService;

        public LoansController(ILoanService loanService)
        {
            _loanService = loanService;
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
                throw new UnauthorizedAccessException("Invalid user ID.");
            return userId;
        }

        // GET: api/loans
        [HttpGet]
        public async Task<ActionResult<List<LoanSummaryDto>>> GetLoans()
        {
            var loans = await _loanService.GetLoansAsync(GetUserId());
            return Ok(loans);
        }

        // GET: api/loans/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<LoanResponseDto>> GetLoan(int id)
        {
            var loan = await _loanService.GetLoanByIdAsync(id, GetUserId());
            if (loan == null) return NotFound();
            return Ok(loan);
        }

        // POST: api/loans
        [HttpPost]
        public async Task<ActionResult<LoanResponseDto>> CreateLoan([FromBody] CreateLoanDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var loan = await _loanService.CreateLoanAsync(dto, GetUserId());
            return CreatedAtAction(nameof(GetLoan), new { id = loan.LoanId }, loan);
        }

        // POST: api/loans/{id}/payments
        [HttpPost("{id}/payments")]
        public async Task<ActionResult<LoanPaymentResponseDto>> CreateLoanPayment(int id, [FromBody] CreateLoanPaymentDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (dto.LoanId != id)
                return BadRequest("LoanId in URL and body must match.");

            var payment = await _loanService.CreateLoanPaymentAsync(dto, GetUserId());
            return Ok(payment);
        }

        // GET: api/loans/{id}/payments
        [HttpGet("{id}/payments")]
        public async Task<ActionResult<List<LoanPaymentDetailDto>>> GetLoanPayments(int id)
        {
            var payments = await _loanService.GetLoanPaymentsAsync(id, GetUserId());
            return Ok(payments);
        }
    }
}