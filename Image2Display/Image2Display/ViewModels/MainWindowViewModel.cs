using Avalonia;
using Avalonia.Markup.Xaml.Styling;
using CommunityToolkit.Mvvm.ComponentModel;
using Image2Display.Views;
using System;
using System.Diagnostics;
using System.Drawing;

namespace Image2Display.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    //存放页面用的
    [ObservableProperty]
    private ViewModelBase _navigationViewContent;

    //各个页面的ViewModel
    [ObservableProperty]
    private ImageConvertViewModel _imageConvert;
    [ObservableProperty]
    private FontConvertViewModel _fontConvert;
    [ObservableProperty]
    private DataViewerViewModel _dataViewer;
    [ObservableProperty]
    private SettingsViewModel _settings;

    public MainWindowViewModel()
    {
        ImageConvert = new ImageConvertViewModel();
        FontConvert = new FontConvertViewModel();
        DataViewer = new DataViewerViewModel();
        Settings = new SettingsViewModel();
        _navigationViewContent = ImageConvert;
    }

    /// <summary>
    /// 切换到指定页面
    /// </summary>
    /// <param name="tag">页面的Tag</param>
    public void ChangePage(string tag)
    {
        NavigationViewContent = tag switch
        {
            "ImageConvert" => ImageConvert,
            "FontConvert" => FontConvert,
            "DataViewer" => DataViewer,
            _ => Settings
        };
    }
}
