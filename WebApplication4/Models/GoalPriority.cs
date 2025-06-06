using System.Text.Json.Serialization;

namespace WebApplication4.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum GoalPriority
    {
        Low,
        Medium,
        High
    }
}
