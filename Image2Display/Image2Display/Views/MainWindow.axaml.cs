using Avalonia.Controls;
using Image2Display.ViewModels;
using System.Diagnostics;

namespace Image2Display.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        MainNavigationView.ItemInvoked += MainNavigationView_ItemInvoked;
    }

    private void MainNavigationView_ItemInvoked(object? sender, FluentAvalonia.UI.Controls.NavigationViewItemInvokedEventArgs e)
    {
        var tag = e.InvokedItemContainer.Tag as string;
        ((MainWindowViewModel)this.DataContext!).ChangePage(tag!);
    }
}