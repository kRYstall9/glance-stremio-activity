using System.Text.Json.Serialization;

namespace GlanceStremioActivity.Models
{
    public class LibraryItem
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = "";
        [JsonPropertyName("name")]
        public string Name { get; set; } = "";
        [JsonPropertyName("type")]
        public string Type { get; set; } = ""; // "movie" o "series"
        [JsonPropertyName("removed")]
        public bool Removed { get; set; }
        [JsonPropertyName("poster")]
        public string? PosterUrl { get; set; }
        [JsonPropertyName("state")]
        public LibraryItemState? State { get; set; }
    }

    public class LibraryItemState
    {
        [JsonPropertyName("lastWatched")]
        public DateTime LastWatched { get; set; }
        [JsonPropertyName("timeWatched")]
        public int TimeWatched { get; set; }
        [JsonPropertyName("flaggedAsWatched")]
        public bool FlaggedAsWatched { get; set; }
        [JsonPropertyName("season")]
        public int? Season { get; set; }
        [JsonPropertyName("episode")]
        public int? Episode { get; set; }
        [JsonPropertyName("duration")]
        public int? TotalShowDuration { get; set; }
        [JsonPropertyName("timeOffset")]
        public int? TimeOffset { get; set; }
    }
}
