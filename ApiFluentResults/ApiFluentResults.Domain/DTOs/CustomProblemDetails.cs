using System.Text.Json.Serialization;

namespace ApiFluentResults.Domain.DTOs
{
    public class CustomProblemDetails
    {
        public string Title { get; set; } = null!;
        public string Code { get; set; } = null!;
        public int? Status { get; set; }
        public string Detail { get; set; } = null!;
        public string Instance { get; set; } = null!;
        public string Type { get; set; } = null!;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IDictionary<string, List<string>>? Errors { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IDictionary<string, object>? Metadata { get; set; }

        public CustomProblemDetails(
            string title,
            int? status,
            string detail,
            string instance,
            string type
        )
        {
            Title = title;
            Status = status;
            Detail = detail;
            Instance = instance;
            Type = type;
        }

        public CustomProblemDetails() { }
    }
}
