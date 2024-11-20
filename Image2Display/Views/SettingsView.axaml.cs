using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Image2Display.Views;

public partial class SettingsView : UserControl
{
    public SettingsView()
    {
        InitializeComponent();
        this.DataContext = new ViewModels.SettingsViewModel();
    }
}