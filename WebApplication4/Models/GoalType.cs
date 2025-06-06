using System.Text.Json.Serialization;

namespace WebApplication4.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum GoalType
    {
        Savings,
        DebtPayment,
        Investment,
        EmergencyFund,
        Other
    }
}
