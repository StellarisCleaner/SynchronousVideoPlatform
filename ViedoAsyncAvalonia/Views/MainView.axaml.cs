using Avalonia.Controls;
using Avalonia.Input;

namespace ViedoAsyncAvalonia.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();

        
    }
    public void SideMenu_PointerEnter(object sender, PointerEventArgs e)
    {
        
    }

    public void SideMenu_PointerLeave(object sender, PointerEventArgs e)
    {
        // 缩起导航栏
        SideMenu.Width = 50; // 假设50是只显示图标的宽度
    }

    private void ListBox_PointerEntered(object? sender, Avalonia.Input.PointerEventArgs e)
    {
        // 展开导航栏
        //SideMenu.Width = 200;
        SideMenu.Classes.Add("expanded");
    }

    private void ListBox_PointerExited_1(object? sender, Avalonia.Input.PointerEventArgs e)
    {
        SideMenu.Classes.Remove("expanded");
        
    }
}
