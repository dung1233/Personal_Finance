using System.Text.Json.Serialization;

namespace WebApplication4.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PriorityLevel
    {
        Low,
        Medium,
        High
    }
}
