using System.Globalization;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;

namespace Emby.Plugin.TopPosters.Providers;

public static class ArtworkUrls
{
    public static IEnumerable<RemoteImageInfo> ForItem(BaseItem item, PluginOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.ApiKey)) yield break;

        BaseItem? owner = item switch
        {
            Season season => season.Series,
            Episode episode => episode.Series,
            Movie or Series => item,
            _ => null
        };
        if (owner is null) yield break;

        var tmdb = owner.GetProviderId(MetadataProviders.Tmdb);
        var imdb = owner.GetProviderId(MetadataProviders.Imdb);
        var idType = !string.IsNullOrWhiteSpace(tmdb) ? "tmdb" : "imdb";
        var id = idType == "tmdb" ? $"{(owner is Movie ? "movie" : "series")}-{tmdb}" : imdb;
        if (string.IsNullOrWhiteSpace(id)) yield break;

        var key = Uri.EscapeDataString(options.ApiKey.Trim());
        if (item is Episode ep)
        {
            if (options.EnableEpisodeThumbnails && ep.ParentIndexNumber is >= 0 && ep.IndexNumber is >= 1)
            {
                var numbers = string.Format(CultureInfo.InvariantCulture, "S{0:D2}E{1:D2}", ep.ParentIndexNumber, ep.IndexNumber);
                yield return Image(ImageType.Primary, $"thumbnail/{Uri.EscapeDataString(id)}/{numbers}.jpg", 1280, 720);
            }
            yield break;
        }

        if (item is Season s)
        {
            if (!options.EnableSeasonPosters || s.IndexNumber is not >= 0) yield break;
            id += "-s" + s.IndexNumber.Value.ToString(CultureInfo.InvariantCulture);
            yield return Image(ImageType.Primary, $"poster/{Uri.EscapeDataString(id)}.jpg", 500, 750);
            yield break;
        }

        if (options.EnablePosters) yield return Image(ImageType.Primary, $"poster/{Uri.EscapeDataString(id)}.jpg", 500, 750);
        if (options.EnableLogos) yield return Image(ImageType.Logo, $"logo/{Uri.EscapeDataString(id)}.png", null, null);
        if (options.EnableBackdrops) yield return Image(ImageType.Backdrop, $"background/{Uri.EscapeDataString(id)}.jpg", 1920, 1080);

        // The device in the TOP Posters dashboard: one for the whole server, which serves the same images to every client.
        RemoteImageInfo Image(ImageType type, string path, int? width, int? height) => new()
        {
            ProviderName = "TOP Posters",
            Type = type,
            Width = width,
            Height = height,
            Url = $"{Plugin.BaseUrl}/{key}/{idType}/{path}?user_agent=emby_client"
        };
    }
}
