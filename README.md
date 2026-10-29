# TOP Posters for Emby

Artwork provider for Emby Server 4.9 and later. Movie and series posters, season
covers, episode thumbnails, transparent title logos and backgrounds come from your
TOP Posters API key. Appearance follows your TOP Posters dashboard. The server
appears there as the **Emby Server** device: its settings apply to everyone who
watches through it, because Emby stores the images and serves the same ones to
every client.

## Install

Emby only installs plugins from its own catalog, so TOP Posters is copied by hand:

1. Download `Emby.Plugin.TopPosters.dll` from the
   [latest release](https://github.com/TOP-Posters/Emby.Plugin.TopPosters/releases/latest).
2. Copy it into the server's `plugins` folder:
   - Docker: `/config/plugins`
   - Linux packages: `/var/lib/emby/plugins`
   - Windows: `%AppData%\Emby-Server\programdata\plugins`
   - macOS: `~/.config/emby-server/plugins`
3. Restart Emby Server. To update, replace the dll and restart again.

## Set up artwork

1. Open **Settings → Plugins → TOP Posters**, enter your API key and choose the
   artwork types. Posters, seasons and episode thumbnails are on by default; logos
   and backgrounds are optional.
2. In each library, open **Manage library → Image fetchers** and enable TOP Posters
   for Movies, Series, Seasons and Episodes. Move it to the top so library scans
   use it first.
3. For items already in the library: **Refresh metadata → Replace existing images**,
   or pick a TOP Posters image from **Edit images**.

Premium is required for episode thumbnails, logos and backgrounds. Styles, badges
and languages are chosen from the
[TOP Posters dashboard](https://top-posters.com/user/dashboard); refresh the
images in Emby to pick up a change.

## Development

```bash
dotnet build Emby.Plugin.TopPosters.csproj -c Release
dotnet run --project Tests/TopPosters.Tests.csproj -c Release
```

The plugin targets `netstandard2.0` with the stable Emby SDK (`MediaBrowser.Server.Core`
4.9.1.90). The tests cover the artwork URLs with no server or network. Pushing a
`v*` tag builds the dll and publishes a GitHub release.
