using MediaBrowser.Common;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Controller.Plugins;
using MediaBrowser.Model.Drawing;

namespace Emby.Plugin.TopPosters;

public class Plugin : BasePluginSimpleUI<PluginOptions>, IHasThumbImage
{
    public const string BaseUrl = "https://api.top-posters.com";

    public static Plugin? Instance { get; private set; }

    public Plugin(IApplicationHost applicationHost) : base(applicationHost)
    {
        Instance = this;
    }

    public override string Name => "TOP Posters";

    public override Guid Id => Guid.Parse("ed72b092-56ac-48b0-995d-b5b9028b469d");

    public override string Description => "Posters, season covers, episode thumbnails, logos and backgrounds from TOP Posters.";

    public PluginOptions Options => GetOptions();

    public ImageFormat ThumbImageFormat => ImageFormat.Png;

    public Stream GetThumbImage()
        => GetType().Assembly.GetManifestResourceStream(GetType().Namespace + ".ThumbImage.png")!;
}
