using System.Text.Json.Serialization;

namespace GlanceStremioActivity.Models
{
    public class StremioAuthResultModel
    {
        [JsonPropertyName("authKey")]
        public string? Token { get; set; }
    }
}
