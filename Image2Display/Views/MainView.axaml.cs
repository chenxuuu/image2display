using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Chrome;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Styling;
using CommunityToolkit.Mvvm.DependencyInjection;
using FluentAvalonia.UI.Controls;
using FluentAvalonia.UI.Windowing;


namespace Image2Display.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
        Utils.SwitchPage = (page) =>
        {
            MainNavigationView.SelectedItem = MainNavigationView.MenuItems[page];
        };
        Utils.MainView = this;
    }
    private void MainNavigationView_SelectionChanged(object? sender, NavigationViewSelectionChangedEventArgs e)
    {
        var tag = (string)((NavigationViewItem)e.SelectedItem).Tag!;
        var type = tag switch
        {
            "ImageConvert" => typeof(Views.ImageConvertView),
            "ImageProcessing" => typeof(Views.ImageProcessingView),
            "FontConvert" => typeof(Views.FontConvertView),
            "DataViewer" => typeof(Views.DataViewerView),
            _ => typeof(Views.SettingsView)
        };
        //var app = Application.Current!.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
        //var mw = app!.MainWindow as Views.MainWindow;
        this.ContentFrame.Navigate(type);
    }
}