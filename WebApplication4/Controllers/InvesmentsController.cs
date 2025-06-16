using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication4.Dtos;
using WebApplication4.Services;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WebApplication4.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class InvestmentsController : ControllerBase
    {
        private readonly IInvestmentService _investmentService;

        public InvestmentsController(IInvestmentService investmentService)
        {
            _investmentService = investmentService;
        }

        // GET: /api/investments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<InvestmentDto>>> GetInvestments()
        {
            var investments = await _investmentService.GetAllAsync();
            return Ok(investments);
        }

        // GET: /api/investments/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<InvestmentDto>> GetInvestment(int id)
        {
            var investment = await _investmentService.GetByIdAsync(id);
            if (investment == null) return NotFound();
            return Ok(investment);
        }

        // POST: /api/investments
        [HttpPost]
        public async Task<ActionResult<InvestmentDto>> CreateInvestment([FromBody] CreateInvestmentDto dto)
        {
            // Lấy UserId từ JWT token
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? throw new Exception("Không tìm thấy thông tin người dùng"));

            var createdInvestment = await _investmentService.CreateAsync(dto, userId);
            return CreatedAtAction(nameof(GetInvestment), new { id = createdInvestment.InvestmentId }, createdInvestment);
        }

        // PUT: /api/investments/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateInvestment(int id, [FromBody] UpdateInvestmentDto dto)
        {
            var success = await _investmentService.UpdateAsync(id, dto);
            if (!success) return NotFound();
            return NoContent();
        }

        // DELETE: /api/investments/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInvestment(int id)
        {
            var success = await _investmentService.DeleteAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }

        // GET: /api/investments/summary
        [HttpGet("summary")]
        public async Task<ActionResult<InvestmentSummaryDto>> GetSummary()
        {
            var summary = await _investmentService.GetSummaryAsync();
            return Ok(summary);
        }
    }
}