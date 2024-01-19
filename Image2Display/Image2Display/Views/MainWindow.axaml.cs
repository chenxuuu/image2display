using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Chrome;
using Avalonia.Markup.Xaml.Styling;
using FluentAvalonia.UI.Controls;
using FluentAvalonia.UI.Windowing;
using Image2Display.ViewModels;
using System;
using System.Diagnostics;

namespace Image2Display.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void MainNavigationView_SelectionChanged(object? sender, FluentAvalonia.UI.Controls.NavigationViewSelectionChangedEventArgs e)
    {
        var tag = (string)((NavigationViewItem)e.SelectedItem).Tag!;
        var type = tag switch
        {
            "ImageConvert" => typeof(ImageConvertView),
            "FontConvert" => typeof(FontConvertView),
            "DataViewer" => typeof(DataViewerView),
            _ => typeof(SettingsView)
        };
        ContentFrame.Navigate(type);
    }
}