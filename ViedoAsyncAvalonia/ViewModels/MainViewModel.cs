using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using LibVLCSharp.Avalonia;
using LibVLCSharp.Shared;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using Tmds.DBus.Protocol;
using ViedoAsyncAvalonia.Util;
using ViedoAsyncAvalonia.Views;
using static ViedoAsyncAvalonia.Util.NetworkUtil;

namespace ViedoAsyncAvalonia.ViewModels;

public class MainViewModel : ViewModelBase, INotifyPropertyChanged
{
    public string Greeting => "这是侧面导航!";

    public VideoViewModel VideoViewModel { get; }


    StartPageViewModel startPageViewModel;
    ActivingRoomsViewModel activingRoomsViewModel;
    VideoViewModel videoViewModel;
    LoginAndRegisterViewModel loginAndRegisterViewModel;



    private void CommandReceived(string command)
    {
        string[] mess = command.Split(" ");
        switch (mess[0])
        {
            case "heartbeat":
                {

                    break;
                }
            case "加入房间成功":
                {
                    break;
                }
            case "创建房间成功":
                {
                    //房间信息获取
                                      
                    break;
                }
            default: { break; }
        }




        switch (command)
        {
            case "stop":
                // 在主UI线程上调用 VideoViewModel 的 StopVideo 方法
                Dispatcher.UIThread.InvokeAsync(VideoViewModel.StopVideo);
                break;
                // 处理其他命令
        }
    }

    // 确保在适当时取消事件订阅以避免内存泄露
    // 例如，在析构函数或关闭逻辑中取消订阅





    public ObservableCollection<MenuItemViewModel> MenuItems { get; }
    private ViewModelBase _currentView;
    public ViewModelBase CurrentView { get => _currentView; 
        set
        {
            this.RaiseAndSetIfChanged(ref _currentView,value);
        }
    }

    public string IPAddress
    {
        get => _iPAddress;
        set {
            this.RaiseAndSetIfChanged(ref _iPAddress, value);
        }
            
    }
    string _iPAddress;
    public string Networtport
    {
        get => _networtport;
        set{
            this.RaiseAndSetIfChanged(ref _networtport, value);
        }
    }
    
    string _networtport;
    RoomInWindowViewModel roomInWindowViewModel = new RoomInWindowViewModel();
    ///获得信息后吧data给Window
    RoomInWindow roomInWindow = new RoomInWindow();
    public MainViewModel(App app)
    {
       

        //初始化网络
        networkUtil = NetworkUtil.Instance;
        NetworkUtil.Instance.CommandReceived += CommandReceived;
        NetworkUtil.Instance.Loginsuccess += LoginIn;
        NetworkUtil.Instance.IntoRoom += OnIntoRoom;
        //networkUtil.ConnToServer();


       


        //InitializeVideo();
        // 初始化菜单项
        startPageViewModel = new StartPageViewModel();
        startPageViewModel.RequestViewChange += HandleViewChange;
        activingRoomsViewModel = new ActivingRoomsViewModel();
        videoViewModel = new VideoViewModel(app);
        loginAndRegisterViewModel = new LoginAndRegisterViewModel();
        MenuItems = new ObservableCollection<MenuItemViewModel>
        {
            new MenuItemViewModel { Name = "初始页面", ViewModel = startPageViewModel },
            new MenuItemViewModel { Name = "房间页面", ViewModel = activingRoomsViewModel },
            new MenuItemViewModel { Name = "视频页面", ViewModel = videoViewModel },
            new MenuItemViewModel { Name = "登录页面", ViewModel = loginAndRegisterViewModel },
            new MenuItemViewModel { Name = "用户头像", ViewModel = loginAndRegisterViewModel, ImageSource = new Bitmap(AssetLoader.Open(new Uri("avares://ViedoAsyncAvalonia/Assets/DefalutProfile.png"))), IsUserAvatar = true }
            // 添加更多页面...
        };
        
        // 默认选中第一个页面
        CurrentView = MenuItems[0].ViewModel;
    }

    private void LoginIn(User obj)
    {
        IsLogin = true;
    }

    private void OnIntoRoom()
    {
        CurrentView = MenuItems[2].ViewModel;
        ///召唤房间界面
        
        /// ///测试
        roomInWindow.DataContext = roomInWindowViewModel;
        roomInWindow.Show();
    }

    private Dictionary<string, string> LoadConfig()
    {
        string configFilePath = Path.Combine(Directory.GetCurrentDirectory(), "app.conf");
        if (!File.Exists(configFilePath))
        {
            // 如果文件不存在，创建一个新的文件并写入初始文本
            using (var writer = new StreamWriter(configFilePath))
            {
                writer.WriteLine("ServerAddress=127.0.0.1");
                writer.WriteLine("ServerPort=15678");
            }
        }
        ///读取配置文件
        Dictionary<string, string> config = new Dictionary<string, string>();
        foreach (var line in File.ReadLines(configFilePath))
        {
            string[] parts = line.Split('=');
            if (parts.Length == 2)
            {
                config[parts[0]] = parts[1];
            }
        }

        // 打印配置项
        foreach (KeyValuePair<string, string> kvp in config)
        {
            Console.WriteLine($"{kvp.Key}={kvp.Value}");
        }
        return config;
        

    }

    private void HandleViewChange(int page)
    {
        // 更改CurrentView为另一个ViewModel实例来切换视图
        CurrentView = MenuItems[page].ViewModel;
        //如果已经登陆并且试图切换到所有活跃房间界面，就发送一个请求数据 获得所有房间的数据
        if(page == 1&&IsLogin)
        {
            NetworkUtil.Instance.Send("GetAllRooms");
        }
        
    }
    private void InitializeVideo()
    {
        Core.Initialize();

        var libVLC = new LibVLC();
        var mediaPlayer = new MediaPlayer(libVLC);
    }

    // 更新SelectedItem的setter来改变CurrentView
    public MenuItemViewModel SelectedItem
    {
        // setter中
        set
        {
            // 省略其它代码
            CurrentView = value.ViewModel;
        }
    }

    public NetworkUtil networkUtil { get; private set; }
    public bool IsLogin { get=>_islogin; set=>this.RaiseAndSetIfChanged(ref _islogin,value); }

    private bool _islogin = false;
}
