using GlanceStremioActivity.DTOs;
using GlanceStremioActivity.Enums;

namespace GlanceStremioActivity.Utils
{
    public static class Utils
    {
        public static string BuildHtml(List<UserActivityResponseDTO> activities, ActivityTypeEnum type)
        {
            var rows = activities.Select(a =>
            {
                var progressPercent = (a.Duration.HasValue && a.TimeWatched.HasValue && a.Duration > 0)
                    ? (int)((double)a.TimeWatched.Value / a.Duration.Value * 100)
                    : (int?)null;

                var progressBar = progressPercent.HasValue && type == ActivityTypeEnum.Watching ? $"""
                    <div style="height:3px; background:var(--color-text-subdue); border-radius:2px; margin-top:4px;">
                        <div style="width:{progressPercent}%; height:100%; background:var(--color-primary); border-radius:2px;"></div>
                    </div>
                """ : "";

                var timeInfo = (a.TimeWatched.HasValue && a.Duration.HasValue && type == ActivityTypeEnum.Watching)
                    ? $"{a.TimeWatched}m / {a.Duration}m"
                    : a.TimeWatched.HasValue && type == ActivityTypeEnum.Watching ? $"{a.TimeWatched}m watched" : "";

                var poster = a.PosterUrl != null ? $"""
                    <img src="{a.PosterUrl}" 
                         style="width:36px; height:54px; object-fit:cover; border-radius:4px; flex-shrink:0;" />
                """ : "";

                return $"""
                    <li style="display:flex; gap:12px; padding:4px 0; list-style:none; border-bottom:1px solid var(--color-border); align-items:flex-start;">
                        {poster}
                        <div style="flex:1; min-width:0;">
                            <div style="display:flex; gap:8px; align-items:center;">
                                <span style="color:var(--color-primary); overflow:hidden; text-overflow:ellipsis; white-space:nowrap; flex:1;">{a.ShowTitle ?? "Unknown"}</span>
                                <span style="color:var(--color-text-subdue); font-size:0.8em; white-space:nowrap; flex-shrink:0;">{a.DisplayName}</span>
                            </div>
                            <span style="color:var(--color-text-subdue); font-size:0.85em;">{timeInfo}</span>
                            {progressBar}
                        </div>
                    </li>
                """;
            });

            return $"""
            <div>
                <ul style="margin:0; padding:0; display:flex; flex-direction:column; gap:4px;">
                    {string.Join("\n", rows)}
                </ul>
            </div>
            """;
        }

        public static string NoActivityHtml(ActivityTypeEnum type) => $"""
            <p style="color:var(--color-text-subdue); text-align:center; padding:8px 0; margin:0;">
                {(type == ActivityTypeEnum.Watching
                    ? "No content playing right now."
                    : "No recent activity found.")}
            </p>
        """;
    }
}