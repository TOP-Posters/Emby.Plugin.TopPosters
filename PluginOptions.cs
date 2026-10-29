using System.ComponentModel;
using Emby.Web.GenericEdit;
using Emby.Web.GenericEdit.Validation;

namespace Emby.Plugin.TopPosters;

public class PluginOptions : EditableOptionsBase
{
    public override string EditorTitle => "TOP Posters";

    public override string EditorDescription =>
        "Artwork from your TOP Posters API key. Get a free key at https://top-posters.com and choose styles, " +
        "badges and languages from your dashboard: Emby appears there as the \"Emby Server\" device, and its " +
        "settings apply to everyone who watches through this server.\n" +
        "Then enable TOP Posters as an image fetcher in each library (Movies, Shows, Seasons, Episodes) and move it " +
        "to the top to use it during scans.";

    [DisplayName("API key")]
    [Description("Your key from the TOP Posters dashboard, for example TP-xxxxxxxxxxxxxxxx.")]
    public string ApiKey { get; set; } = string.Empty;

    [DisplayName("Movie and series posters")]
    public bool EnablePosters { get; set; } = true;

    [DisplayName("Season posters")]
    public bool EnableSeasonPosters { get; set; } = true;

    [DisplayName("Episode thumbnails")]
    [Description("Requires the Premium plan.")]
    public bool EnableEpisodeThumbnails { get; set; } = true;

    [DisplayName("Title logos")]
    [Description("Transparent logos. Requires the Premium plan.")]
    public bool EnableLogos { get; set; }

    [DisplayName("Backgrounds")]
    [Description("Requires the Premium plan.")]
    public bool EnableBackdrops { get; set; }

    protected override void Validate(ValidationContext context)
    {
        var key = ApiKey?.Trim() ?? string.Empty;
        if (key.Length > 0 && (key.Length > 128 || key.Any(char.IsWhiteSpace)))
            context.AddValidationError(nameof(ApiKey), "Paste the key exactly as shown in the dashboard.");
    }
}
