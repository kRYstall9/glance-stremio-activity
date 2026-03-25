using System.Text.Json.Serialization;

namespace GlanceStremioActivity.Models
{
    public class StremioAuthModel
    {
        [JsonPropertyName("result")]
        public StremioAuthResultModel? Result { get; set; }
    }
    
}
