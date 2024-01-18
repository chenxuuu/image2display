using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Image2Display.Views;

public partial class FontConvertView : UserControl
{
    public FontConvertView()
    {
        InitializeComponent();
        this.DataContext = new ViewModels.FontConvertViewModel();
    }
}