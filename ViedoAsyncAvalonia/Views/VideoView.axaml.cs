using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using LibVLCSharp.Shared;
using ReactiveUI;
using System;
using System.IO;
using System.Reactive.Linq;
using ViedoAsyncAvalonia.ViewModels;
namespace ViedoAsyncAvalonia.Views;

public partial class VideoView : UserControl
{
    private readonly DispatcherTimer _hideControlsTimer;

    public VideoView()
    {
        InitializeComponent();

        //InitializeVideoPlayer();
        //DataContext = new VideoViewModel();
        controlPanel.ZIndex = +5;
        _hideControlsTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(3),
        };
        _hideControlsTimer.Tick += (sender, e) =>
        {
            //controlPanel.Opacity = 0; // 渐隐控制面板
        };
        
        //当鼠标在VideoView上移动时重置计时器
        this.AddHandler(PointerMovedEvent, (sender, e) =>
        {
            controlPanel.Opacity = 1;
            _hideControlsTimer.Stop();
            _hideControlsTimer.Start();
        }, RoutingStrategies.Tunnel); // 或者使用 RoutingStrategies.Bubble 根据需要
        _hideControlsTimer.Start();
    }
    // 确保在适当的时候取消事件订阅，避免内存泄漏
    //protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    //{
    //    base.OnDetachedFromVisualTree(e);
    //    if (DataContext is VideoViewModel viewModel)
    //    {
    //        viewModel.MediaPlayerChanged -= OnMediaPlayerChanged;
    //    }
    //}

    private async void OpenFileButton_Clicked(object sender, RoutedEventArgs args)
    {
        // 从当前控件获取 TopLevel。或者，您也可以使用 Window 引用。
        var topLevel = TopLevel.GetTopLevel(this);

        // 启动异步操作以打开对话框。
        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open Text File",
            AllowMultiple = false
        });

        if (files.Count >= 1)
        {
            Uri path = files[0].Path;
            if (DataContext is VideoViewModel viewModel)
            {
                viewModel.VideoUrl = path;
            }


            //// 打开第一个文件的读取流。
            //await using var stream = await files[0].OpenReadAsync();
            //using var streamReader = new StreamReader(stream);
            //// 将文件的所有内容作为文本读取。
            //var fileContent = await streamReader.ReadToEndAsync();
        }
    }
}