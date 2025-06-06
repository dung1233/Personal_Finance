using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApplication4.Models;
using WebApplication4.Services;

namespace WebApplication4.Controllers
{
    [ApiController]
    [Route("api/dashboard")]
    [Authorize] // ✅ bắt buộc user phải đăng nhập
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("overview")]
        public async Task<IActionResult> GetOverview()
        {
            var userId = GetUserId();
            Console.WriteLine($"[DEBUG] UserId from JWT: {userId}"); // Debug
            var result = await _dashboardService.GetOverviewAsync(userId);
            return Ok(result);
        }

        private int GetUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        }
        [HttpGet("accounts-summary")]
        public async Task<IActionResult> GetAccountsSummary()
        {
            var userId = GetUserId();
            var result = await _dashboardService.GetAccountsSummaryAsync(userId);
            return Ok(result);
        }


    }



}
