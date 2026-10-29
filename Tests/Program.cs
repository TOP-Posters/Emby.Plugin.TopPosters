using System.Reflection;
using Emby.Plugin.TopPosters;
using Emby.Plugin.TopPosters.Providers;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Library;
using MediaBrowser.Model.Entities;

var passed = 0;
var options = new PluginOptions { ApiKey = " TP-test " };
Check(options.EnablePosters && options.EnableSeasonPosters && options.EnableEpisodeThumbnails && !options.EnableLogos && !options.EnableBackdrops, "posters, seasons and thumbnails on by default; logos and backgrounds opt-in");

var movie = new Movie { Name = "Movie" };
movie.SetProviderId(MetadataProviders.Tmdb, "550");
movie.SetProviderId(MetadataProviders.Imdb, "tt0137523");
var images = ArtworkUrls.ForItem(movie, options).ToArray();
Check(images.Length == 1 && images[0].Url == "https://api.top-posters.com/TP-test/tmdb/poster/movie-550.jpg?user_agent=emby_client", "movie uses the typed TMDB ID, the trimmed key and the Emby Server device");
options.EnableLogos = options.EnableBackdrops = true;
images = ArtworkUrls.ForItem(movie, options).ToArray();
Check(images.Select(i => i.Type).SequenceEqual(new[] { ImageType.Primary, ImageType.Logo, ImageType.Backdrop }) && images[1].Url.Contains("/logo/movie-550.png") && images[2].Url.Contains("/background/movie-550.jpg"), "optional artwork uses the matching PNG and JPEG endpoints");
movie.ProviderIds.Remove("Tmdb");
Check(ArtworkUrls.ForItem(movie, options).First().Url.Contains("/imdb/poster/tt0137523.jpg"), "IMDb fallback works without TMDB");
movie.ProviderIds.Clear();
Check(!ArtworkUrls.ForItem(movie, options).Any(), "missing identifiers produce no unusable image URLs");

var series = new Series { Name = "Series", InternalId = 10 };
series.SetProviderId(MetadataProviders.Tmdb, "1396");
BaseItem.LibraryManager = Stub.Create<ILibraryManager>((method, args) => method.Name == "GetItemById" && args?[0] is long id && id == 10 ? series : null);
var season = new Season { InternalId = 11, SeriesId = 10, IndexNumber = 0 };
var episode = new Episode { InternalId = 12, SeriesId = 10, ParentIndexNumber = 0, IndexNumber = 1 };
Check(ArtworkUrls.ForItem(season, options).Single().Url.Contains("/tmdb/poster/series-1396-s0.jpg"), "season zero uses the parent series ID and the season suffix");
Check(ArtworkUrls.ForItem(episode, options).Single().Url.Contains("/tmdb/thumbnail/series-1396/S00E01.jpg"), "specials use valid season-zero episode numbering");
episode.IndexNumber = null;
Check(!ArtworkUrls.ForItem(episode, options).Any(), "episodes with missing numbers are skipped");
options.EnableSeasonPosters = false;
Check(!ArtworkUrls.ForItem(season, options).Any(), "season artwork respects its switch");
options.ApiKey = " ";
Check(!ArtworkUrls.ForItem(series, options).Any(), "blank keys never produce image candidates");

Console.WriteLine($"Passed {passed} checks.");

void Check(bool condition, string name)
{
    if (!condition) throw new Exception("FAIL: " + name);
    Console.WriteLine("PASS: " + name);
    passed++;
}

public class Stub : DispatchProxy
{
    public Func<MethodInfo, object?[]?, object?> Handler { get; set; } = null!;
    protected override object? Invoke(MethodInfo? method, object?[]? args)
    {
        var result = Handler(method!, args);
        if (result is not null || method!.ReturnType == typeof(void)) return result;
        return method.ReturnType.IsValueType ? Activator.CreateInstance(method.ReturnType) : null;
    }
    public static T Create<T>(Func<MethodInfo, object?[]?, object?> handler) where T : class
    {
        var proxy = Create<T, Stub>();
        ((Stub)(object)proxy).Handler = handler;
        return proxy;
    }
}
