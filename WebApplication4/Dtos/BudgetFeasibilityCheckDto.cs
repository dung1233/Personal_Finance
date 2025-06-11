namespace WebApplication4.Dtos
{
    public class BudgetFeasibilityCheckDto
    {
        public bool IsFeasible { get; set; }
        public List<string> Warnings { get; set; } = new List<string>();
        public List<string> Suggestions { get; set; } = new List<string>();
    }
}
