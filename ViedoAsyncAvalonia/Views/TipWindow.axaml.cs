using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace ViedoAsyncAvalonia.Views;

public partial class TipWindow : Window
{
    public TipWindow(string message)
    {
        InitializeComponent();
        Message.Text = message;
    }
    private void OnOkClicked(object sender, RoutedEventArgs e)
    {
        this.Close();
    }
}