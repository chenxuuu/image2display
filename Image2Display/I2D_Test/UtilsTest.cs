using Avalonia;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Image2Display;
using Image2Display.ViewModels;
using Image2Display.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace I2D_Test
{
    public class UtilsTest
    {
        public static AppBuilder BuildAvaloniaApp() => AppBuilder.Configure<App>()
            .UseHeadless(new AvaloniaHeadlessPlatformOptions());

        [AvaloniaFact]
        public void Language()
        {
            Utils.Initial();
            _ = new MainWindow()
            {
                DataContext = new MainWindowViewModel(),
            };
            Utils.ChangeLanguage("test",true);
            Utils.ChangeLanguage("en-US", true);
        }

        [AvaloniaFact]
        public void Setting()
        {
            Utils.Initial();
            _ = new MainWindow()
            {
                DataContext = new MainWindowViewModel(),
            };
            Utils.Settings.Language = "en-US";
            Utils.SaveSettings();
            Utils.Settings.Language = "zh-CN";
            Utils.SaveSettings();
            Assert.Equal("zh-CN", Utils.Settings.Language);
        }
    }
}
