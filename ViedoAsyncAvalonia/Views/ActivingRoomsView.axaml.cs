using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ViedoAsyncAvalonia.ViewModels;

namespace ViedoAsyncAvalonia.Views;

public partial class ActivingRoomsView : UserControl
{
    public ActivingRoomsView()
    {
        InitializeComponent();
        DataContext = new ActivingRoomsViewModel();
        
    }
}