using System.Collections.Generic;

namespace WebApplication4.Models
{
    public class BulkCategoryRequest
    {
        public List<CategoryCreateRequest> Categories { get; set; } = new();
    }

    public class CategoryCreateRequest
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string CategoryType { get; set; } = "Expense";
        public string? Color { get; set; }
        public string? Icon { get; set; }
        public bool IsDefault { get; set; } = false;
    }
}
