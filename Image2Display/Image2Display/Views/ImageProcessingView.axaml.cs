using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Image2Display.ViewModels;
using System.Diagnostics;

namespace Image2Display.Views;

public partial class ImageProcessingView : UserControl
{
    public ImageProcessingView()
    {
        InitializeComponent();
        this.DataContext = new ViewModels.ImageProcessingViewModel();
    }

    private void Grid_PointerWheelChanged(object? sender, Avalonia.Input.PointerWheelEventArgs e)
    {
        // ���������¼�
        var delta = e.Delta.Y;
        Debug.WriteLine($"delta: {delta}");
        var mar = ImageViewbox.Margin.Left;
        var marAfter = mar - (delta * 10);
        if (marAfter >= 10)
            return;
        ImageViewbox.Margin = new Thickness(marAfter);
    }
}