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
            //controlPanel.Opacity = 0; // �����������
        };
        
        //�������VideoView���ƶ�ʱ���ü�ʱ��
        this.AddHandler(PointerMovedEvent, (sender, e) =>
        {
            controlPanel.Opacity = 1;
            _hideControlsTimer.Stop();
            _hideControlsTimer.Start();
        }, RoutingStrategies.Tunnel); // ����ʹ�� RoutingStrategies.Bubble ������Ҫ
        _hideControlsTimer.Start();
    }
    // ȷ�����ʵ���ʱ��ȡ���¼����ģ������ڴ�й©
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
        // �ӵ�ǰ�ؼ���ȡ TopLevel�����ߣ���Ҳ����ʹ�� Window ���á�
        var topLevel = TopLevel.GetTopLevel(this);

        // �����첽�����Դ򿪶Ի���
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


            //// �򿪵�һ���ļ��Ķ�ȡ����
            //await using var stream = await files[0].OpenReadAsync();
            //using var streamReader = new StreamReader(stream);
            //// ���ļ�������������Ϊ�ı���ȡ��
            //var fileContent = await streamReader.ReadToEndAsync();
        }
    }
}