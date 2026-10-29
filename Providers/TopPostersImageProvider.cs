using MediaBrowser.Common.Net;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Configuration;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;

namespace Emby.Plugin.TopPosters.Providers;

// Emby finds the provider by reflection and injects the constructor arguments.
public class TopPostersImageProvider(IHttpClient httpClient) : IRemoteImageProvider, IHasOrder
{
    public string Name => "TOP Posters";

    public int Order => 0;

    public bool Supports(BaseItem item) => item is Movie or Series or Season or Episode;

    public IEnumerable<ImageType> GetSupportedImages(BaseItem item)
    {
        var options = Plugin.Instance?.Options;
        if (item is Season or Episode)
        {
            yield return ImageType.Primary;
            yield break;
        }
        if (options?.EnablePosters != false) yield return ImageType.Primary;
        if (options?.EnableLogos == true) yield return ImageType.Logo;
        if (options?.EnableBackdrops == true) yield return ImageType.Backdrop;
    }

    public Task<IEnumerable<RemoteImageInfo>> GetImages(BaseItem item, LibraryOptions libraryOptions, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var options = Plugin.Instance?.Options;
        return Task.FromResult(options is null || !Supports(item)
            ? Enumerable.Empty<RemoteImageInfo>()
            : ArtworkUrls.ForItem(item, options).ToArray().AsEnumerable());
    }

    public Task<HttpResponseInfo> GetImageResponse(string url, CancellationToken cancellationToken)
        => httpClient.GetResponse(new HttpRequestOptions { Url = url, CancellationToken = cancellationToken });
}
