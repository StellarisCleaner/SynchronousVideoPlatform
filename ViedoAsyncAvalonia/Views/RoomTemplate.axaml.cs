using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ViedoAsyncAvalonia.Util;

namespace ViedoAsyncAvalonia.Views;

public partial class RoomTemplate : UserControl
{
    public RoomTemplate()
    {
        InitializeComponent();
    }

    private void Border_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        if (DataContext is Room room)
        {
            room.ClickOn();
            ///出现加载画面
            ///五秒之后连接失败
        }
    }
}