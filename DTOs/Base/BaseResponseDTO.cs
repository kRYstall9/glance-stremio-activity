using System.Text.Json.Serialization;

namespace GlanceStremioActivity.DTOs.Base
{
    public class BaseResponseDTO
    {
        public bool Success { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Error { get; set; }
    }
}
