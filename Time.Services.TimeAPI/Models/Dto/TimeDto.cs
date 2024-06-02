using System.Text.Json.Serialization;

namespace Time.Services.TimeAPI.Models.Dto
{
    public class TimeDto
    {
        [JsonPropertyName("currentTime")]
        public DateTime? CurrentTime { get; set; }

        [JsonPropertyName("adjustedTime")]
        public DateTime? AdjustedTime { get; set; }
    }
}
