using Avalonia.Headless;
using Avalonia;
using Image2Display;
using I2D_Test;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Controls.ApplicationLifetimes;
using Image2Display.ViewModels;
using Avalonia.Metadata;

[assembly: AvaloniaTestApplication(typeof(MainWindow))]
namespace I2D_Test
{
    public class MainWindow
    {
        public static AppBuilder BuildAvaloniaApp() => AppBuilder.Configure<App>()
            .UseHeadless(new AvaloniaHeadlessPlatformOptions());

        [AvaloniaFact]
        public void Should_Type_Text_Into_TextBox()
        {
            // Setup controls:
            var textBox = new TextBox();
            var window = new Window { Content = textBox };

            // Open window:
            window.Show();

            // Focus text box:
            textBox.Focus();

            // Simulate text input:
            window.KeyTextInput("Hello World");

            // Assert:
            Assert.Equal("Hello World", textBox.Text);
        }

        [Fact]
        public void ChangePage()
        {
            var mvm = new MainWindowViewModel();
            mvm.ChangePage("Settings");
            mvm.ChangePage("FontConvert");
            mvm.ChangePage("DataViewer");
            mvm.ChangePage("ImageConvert");

        }
    }

}