using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ViedoAsyncAvalonia.ViewModels;

namespace ViedoAsyncAvalonia.Views;

public partial class CreateRoomWindow : Window
{
    public CreateRoomWindow()
    {
        InitializeComponent();
        DataContext = new CreateRoomViewModel();
    }
}