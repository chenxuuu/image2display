using Avalonia.Headless;
using Avalonia;
using Image2Display;
using I2D_Test;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Controls.ApplicationLifetimes;
using Image2Display.ViewModels;
using Avalonia.Metadata;
using Image2Display.Views;

[assembly: AvaloniaTestApplication(typeof(MainWindowTest))]
namespace I2D_Test
{
    public class MainWindowTest
    {
        public static AppBuilder BuildAvaloniaApp() => AppBuilder.Configure<App>()
            .UseHeadless(new AvaloniaHeadlessPlatformOptions());

        [AvaloniaFact]
        public void ChangePage()
        {
            Utils.Initial();
            var mvm = new MainWindowViewModel();
            _ = new MainWindow()
            {
                DataContext = mvm,
            };
            string[] pageList = new string[] { "ImageConvert", "FontConvert", "DataViewer", "Settings" };
            for (int i = 0; i < 10; i++)
                for (int j = 0; j < pageList.Length; j++)
                    mvm.ChangePage(pageList[j]);
        }
    }

}