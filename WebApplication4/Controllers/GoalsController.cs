using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WebApplication4.Dtos;
using WebApplication4.Services;
using WebApplication4.Models;
using WebApplication4.Data; // Thêm để sử dụng AppDbContext
using System.Security.Claims;
using Microsoft.Extensions.Logging;
namespace WebApplication4.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class GoalsController : ControllerBase
    {
        private readonly IGoalService _goalService;
        private readonly AppDbContext _context; // Sử dụng AppDbContext
        private readonly ILogger<GoalsController> _logger;
        public GoalsController(IGoalService goalService, AppDbContext context, ILogger<GoalsController> logger)
        {
            _goalService = goalService;
            _context = context;
            _logger = logger; // Fix: Assign the logger parameter to the _logger field
        }

        private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        // GET /api/goals
        [HttpGet]
        public async Task<IActionResult> GetGoals()
        {
            var goals = await _goalService.GetGoalsAsync(GetUserId());
            return Ok(goals);
        }

        // GET /api/goals/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetGoal(int id)
        {
            var goal = await _goalService.GetGoalByIdAsync(GetUserId(), id);
            if (goal == null) return NotFound();
            return Ok(goal);
        }

        // POST /api/goals
        [HttpPost]
        public async Task<IActionResult> CreateGoal([FromBody] CreateGoalDto dto)
        {
            var goal = await _goalService.CreateGoalAsync(GetUserId(), dto);
            return CreatedAtAction(nameof(GetGoal), new { id = goal.GoalId }, goal);
        }

        // PUT /api/goals/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGoal(int id, [FromBody] UpdateGoalDto dto)
        {
            var goal = await _goalService.UpdateGoalAsync(GetUserId(), id, dto);
            if (goal == null) return NotFound();
            return Ok(goal);
        }

        // DELETE /api/goals/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGoal(int id)
        {
            var result = await _goalService.DeleteGoalAsync(GetUserId(), id);
            return result ? NoContent() : NotFound();
        }

        // PATCH /api/goals/{id}/complete
        [HttpPatch("{id}/complete")]
        public async Task<IActionResult> CompleteGoal(int id)
        {
            var success = await _goalService.MarkGoalCompleteAsync(GetUserId(), id);
            return success ? Ok(new { message = "Goal marked as completed." }) : NotFound();
        }

        // PATCH /api/goals/{id}/activate
        [HttpPatch("{id}/activate")]
        public async Task<IActionResult> ToggleGoalActive(int id)
        {
            var success = await _goalService.ToggleGoalActiveAsync(GetUserId(), id);
            return success ? Ok(new { message = "Goal activation status toggled." }) : NotFound();
        }

        // GET /api/goals/summary
        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary()
        {
            var summary = await _goalService.GetGoalSummaryAsync(GetUserId());
            return Ok(summary);
        }

        


        [HttpGet("{id}/progress")]
        public async Task<IActionResult> GetGoalProgress(int id)
        {
            var userId = GetUserId();
            var goal = await _context.Goals.FirstOrDefaultAsync(g => g.GoalId == id && g.UserId == userId);
            if (goal == null || goal.TargetAmount == 0) return NotFound();

            var progress = (goal.CurrentAmount / goal.TargetAmount) * 100;
            return Ok(new { GoalId = goal.GoalId, Progress = Math.Round(progress, 2) });
        }

        [HttpPost("bulk")]
        public async Task<IActionResult> CreateGoalsBulk([FromBody] List<CreateGoalDto> goals)
        {
            var userId = GetUserId(); // Thêm UserId cho mỗi goal

            var goalEntities = goals.Select(dto => new Goal
            {
                UserId = userId, // Thêm UserId
                GoalName = dto.GoalName,
                Description = dto.Description,
                GoalType = dto.GoalType,
                TargetAmount = dto.TargetAmount,
                CurrentAmount = 0,
                TargetDate = dto.TargetDate,
                Priority = dto.Priority,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }).ToList();

            await _context.Goals.AddRangeAsync(goalEntities);
            await _context.SaveChangesAsync();
            return Ok(goalEntities);
        }

        [HttpGet("alerts")]
        public async Task<IActionResult> GetGoalAlerts()
        {
            var userId = GetUserId();
            var now = DateTime.UtcNow;
            var soon = now.AddDays(7);

            var alerts = await _context.Goals
                .Where(g => g.UserId == userId && g.IsActive && !g.IsCompleted && g.TargetDate <= soon)
                .ToListAsync();

            return Ok(alerts);
        }

        [HttpGet("priorities")]
        public async Task<IActionResult> GetGoalsByPriority([FromQuery] string level)
        {
            var userId = GetUserId();

            // Parse string thành GoalPriority enum
            if (!Enum.TryParse<GoalPriority>(level, true, out var priority))
            {
                return BadRequest("Invalid priority level. Valid values: Low, Medium, High");
            }

            var goals = await _context.Goals
                .Where(g => g.UserId == userId && g.Priority == priority && g.IsActive)
                .ToListAsync();

            return Ok(goals);
        }
        [HttpGet("/api/contributions")]
        public async Task<IActionResult> GetAllContributions()
        {
            var userId = GetUserId();

            var contributions = await _context.GoalContributions
                .Where(c => c.Goal.UserId == userId)
                .Include(c => c.Goal)
                .OrderByDescending(c => c.ContributionDate)
                .Select(c => new
                {
                    c.ContributionId,
                    c.GoalId,
                    GoalName = c.Goal.GoalName,
                    c.Amount,
                    c.ContributionDate,
                    c.Notes,
                    c.CreatedAt
                })
                .ToListAsync();

            return Ok(contributions);
        }
        [HttpPost("{id}/contributions")]
        public async Task<IActionResult> ContributeToGoal(int id, [FromBody] ContributionDto dto)
        {
            var userId = GetUserId();

            var goal = await _context.Goals.FirstOrDefaultAsync(g => g.GoalId == id && g.UserId == userId);
            if (goal == null || !goal.IsActive) return NotFound();

            // ✅ Gán ngày đóng góp nếu không được truyền
            var contributionDate = dto.ContributionDate == default ? DateTime.UtcNow : dto.ContributionDate;

            var contribution = new GoalContribution
            {
                GoalId = goal.GoalId,
                Amount = dto.Amount,
                ContributionDate = contributionDate,
                Notes = dto.Note,
                CreatedAt = DateTime.UtcNow
            };

            _context.GoalContributions.Add(contribution);

            // ✅ Cập nhật tiến độ của mục tiêu
            goal.CurrentAmount += dto.Amount;
            goal.UpdatedAt = DateTime.UtcNow;

            if (goal.CurrentAmount >= goal.TargetAmount)
            {
                goal.IsCompleted = true;
                goal.CompletedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            try
            {
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    Message = "Đóng góp thành công",
                    Contribution = new
                    {
                        contribution.GoalId,
                        contribution.Amount,
                        contribution.ContributionDate,
                        contribution.Notes
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi thêm contribution");
                return StatusCode(500, "Có lỗi xảy ra");
            }

        }

        [HttpPut("/api/contributions/{id}")]
        public async Task<IActionResult> UpdateContribution(int id, [FromBody] ContributionDto dto)
        {
            var userId = GetUserId();

            var contribution = await _context.GoalContributions
                .Include(c => c.Goal)
                .FirstOrDefaultAsync(c => c.ContributionId == id && c.Goal.UserId == userId);

            if (contribution == null) return NotFound();

            var oldAmount = contribution.Amount;
            var goal = contribution.Goal;

            // ✅ Cập nhật số dư mục tiêu dựa trên chênh lệch
            var delta = dto.Amount - oldAmount;
            goal.CurrentAmount += delta;

            // ✅ Nếu cần, cập nhật trạng thái hoàn thành
            if (goal.CurrentAmount >= goal.TargetAmount)
            {
                goal.IsCompleted = true;
                goal.CompletedAt ??= DateTime.UtcNow;
            }
            else
            {
                goal.IsCompleted = false;
                goal.CompletedAt = null;
            }

            goal.UpdatedAt = DateTime.UtcNow;

            // ✅ Cập nhật đóng góp
            contribution.Amount = dto.Amount;
            contribution.ContributionDate = dto.ContributionDate;
            contribution.Notes = dto.Note;

            await _context.SaveChangesAsync();

            return Ok(new { Message = "Đã cập nhật đóng góp", Contribution = contribution });
        }
        

        [HttpDelete("/api/contributions/{id}")]
        public async Task<IActionResult> DeleteContribution(int id)
        {
            var result = await _goalService.DeleteContributionAsync(id);
            if (!result) return NotFound();

            return NoContent();
        }

        [HttpGet("{id}/analytics")]
        public async Task<IActionResult> GetGoalAnalytics(int id)
        {
            var userId = GetUserId();

            var goal = await _context.Goals
                .Where(g => g.GoalId == id && g.UserId == userId && g.IsActive)
                .FirstOrDefaultAsync();

            if (goal == null) return NotFound();

            var contributions = await _context.GoalContributions
                .Where(c => c.GoalId == id)
                .OrderByDescending(c => c.ContributionDate)
                .ToListAsync();

            var lastContributionDate = contributions.Any()
                ? contributions.First().ContributionDate
                : (DateTime?)null;

            var analytics = new
            {
                goalId = goal.GoalId,
                goalName = goal.GoalName,
                targetAmount = goal.TargetAmount,
                currentAmount = goal.CurrentAmount,
                progressPercentage = goal.TargetAmount > 0
                    ? (goal.CurrentAmount / goal.TargetAmount) * 100
                    : 0,
                contributionCount = contributions.Count,
                lastContributionDate = lastContributionDate ?? DateTime.MinValue,
                remainingAmount = goal.TargetAmount - goal.CurrentAmount
            };

            return Ok(analytics);
        }

        [HttpGet("statistics")]
        public async Task<IActionResult> GetGoalStatistics()
        {
            var userId = GetUserId();
            var result = await _goalService.GetGoalStatisticsAsync(userId);
            return Ok(result);
        }






    }
}