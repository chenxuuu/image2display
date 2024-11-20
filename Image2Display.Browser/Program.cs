using System;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Browser;
using Avalonia.Media.Fonts;
using Image2Display;

internal sealed partial class Program
{
    private static Task Main(string[] args) => BuildAvaloniaApp()
            .StartBrowserAppAsync("out");

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>().ConfigureFonts(manager =>
        {
            manager.AddFontCollection(new EmbeddedFontCollection(new Uri("fonts:App", UriKind.Absolute),
                new Uri("avares://Image2Display/Resources", UriKind.Absolute)));
        });
}