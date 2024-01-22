using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Chrome;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Styling;
using CommunityToolkit.Mvvm.DependencyInjection;
using FluentAvalonia.UI.Controls;
using FluentAvalonia.UI.Windowing;
using Image2Display.Models;
using Image2Display.ViewModels;
using System;
using System.Diagnostics;

namespace Image2Display.Views;

public partial class MainWindow : AppWindow
{
    public MainWindow()
    {
        //恢复设置的主题色
        Application.Current!.RequestedThemeVariant = Utils.Settings.Theme switch
        {
            1 => ThemeVariant.Light,
            2 => ThemeVariant.Dark,
            _ => ThemeVariant.Default,
        };
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