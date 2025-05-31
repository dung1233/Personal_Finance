using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebApplication4.Data;
using WebApplication4.Models;

namespace WebApplication4.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoriesController(AppDbContext context)
        {
            _context = context;
        }

        // GET /api/categories
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var cats = await _context.Categories
                .Where(c => c.UserId == userId && c.IsActive)
                .ToListAsync();
            return Ok(cats);
        }

        // GET /api/categories/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var cat = await _context.Categories
                .FirstOrDefaultAsync(c => c.CategoryId == id && c.UserId == userId);
            return cat is not null ? Ok(cat) : NotFound();
        }

        // POST /api/categories
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CategoryCreateRequest req)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var cat = new Category
            {
                UserId = userId,
                Name = req.Name,
                Description = req.Description,
                CategoryType = req.CategoryType,
                Color = req.Color,
                Icon = req.Icon,
                IsDefault = req.IsDefault
            };
            _context.Categories.Add(cat);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = cat.CategoryId }, cat);
        }

        // PUT /api/categories/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CategoryCreateRequest req)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var cat = await _context.Categories.FindAsync(id);
            if (cat == null || cat.UserId != userId) return NotFound(new { message = "Không tìm thấy danh mục hoặc không thuộc quyền của bạn" });


            cat.Name = req.Name;
            cat.Description = req.Description;
            cat.CategoryType = req.CategoryType;
            cat.Color = req.Color;
            cat.Icon = req.Icon;
            cat.IsDefault = req.IsDefault;
            cat.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Cập nhật thành công", category = cat });
        }

        // DELETE /api/categories/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var cat = await _context.Categories.FindAsync(id);
            if (cat == null || cat.UserId != userId) return NotFound(new { message = "Không tìm thấy danh mục" });

            _context.Categories.Remove(cat);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Đã xóa  thành công", category = cat });
        }

        // GET /api/categories/default
        [AllowAnonymous]
        [HttpGet("default")]
        public async Task<IActionResult> GetDefault()
        {
            var defaults = await _context.Categories
                .Where(c => c.IsDefault && c.IsActive)
                .ToListAsync();
            return Ok(defaults);
        }

        // POST /api/categories/bulk
        [HttpPost("bulk")]
        public async Task<IActionResult> BulkCreate([FromBody] BulkCategoryRequest req)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var list = req.Categories.Select(r => new Category
            {
                UserId = userId,
                Name = r.Name,
                Description = r.Description,
                CategoryType = r.CategoryType,
                Color = r.Color,
                Icon = r.Icon,
                IsDefault = r.IsDefault
            }).ToList();

            _context.Categories.AddRange(list);
            await _context.SaveChangesAsync();
            return Ok(list);
        }
    }
}
