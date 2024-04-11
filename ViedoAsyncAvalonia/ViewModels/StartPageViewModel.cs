using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Newtonsoft.Json;
using NP.Utilities;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ViedoAsyncAvalonia.Util;

namespace ViedoAsyncAvalonia.ViewModels
{
    
    public class StartPageViewModel:ViewModelBase
    {
        private int temp = 2;
        public string IPAddress
        {
            get => _iPAddress;
            set
            {
                this.RaiseAndSetIfChanged(ref _iPAddress, value);
                //ServerInfoChanged?.Invoke(_iPAddress, _networtport);
            }

        }
        string _iPAddress = "乐乐乐";
        public string Networtport
        {
            get => _networtport;
            set
            {
                this.RaiseAndSetIfChanged(ref _networtport, value);
                //ServerInfoChanged?.Invoke(_iPAddress, _networtport);
            }
        }
        string _networtport = "哈哈哈";
        public event Action<int> RequestViewChange;
        public string Title { get; set; }
        public bool IsLogin 
        { get => _isLogin;
                set {
                this.RaiseAndSetIfChanged(ref _isLogin,value);
            } 
        }
        public bool _isLogin  = false;

        public string Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name,value);
        }
        private string _name = "没有登陆";
        public string uuid
        {
            get => _uuid;
            set => this.RaiseAndSetIfChanged(ref _uuid, value);
        }
        private string _uuid = "所以没有uuid";
        public StartPageViewModel() 
        {
            Dictionary<string, string> config = LoadConfig();
            ///赋值网络位置变量
            IPAddress = config["ServerAddress"];
            Networtport = config["ServerPort"];
            //string v = "avares://ViedoAsyncAvalonia/Assets/DefalutProfile.png";
            //ImageSource = new Bitmap(AssetLoader.Open(new Uri(v)));
            //ImageSource = "/Assets/DefalutProfile.png";
            CreateRoomCommand = ReactiveCommand.Create(CreateRoom);
            JoinRoomCommand = ReactiveCommand.Create(JoinRoom);
            AnonymousLoginRequest = ReactiveCommand.Create(AnonymousLogin);
            ConnectToserverCommand = ReactiveCommand.Create(ConnectToserverAsync);
            NetworkUtil.Instance.CommandReceived +=CommandProcess;
            NetworkUtil.Instance.Loginsuccess += LoginChange;
        }

        private void LoginChange(User localuser)
        {
            IsLogin = true;
            uuid = localuser.uuid;
            Name = localuser.username;
            ImageSource = localuser.Avatar;
        }

        private void CommandProcess(string msg)
        {
            if (msg.StartsWith("getSelfProfile"))
            {
                User? SelfUser = JsonConvert.DeserializeObject<User>(msg.Split(" ")[1]);
                Name = SelfUser.username;
                
            }
        }

        private void ConnectToserverAsync()
        {
            NetworkUtil.Instance.ConnToServer(IPAddress, Networtport);
        }

        public ReactiveCommand<Unit, Unit> AnonymousLoginRequest { get; }
        public ReactiveCommand<Unit, Unit> ConnectToserverCommand { get; }
        
        private  void AnonymousLogin()
        {
            NetworkUtil.Instance.Send($"anonymouslogin");
            
        }

        private void JoinRoom()
        {
            RequestViewChange?.Invoke(1);
        }

        private void CreateRoom()
        {
            
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
        private Bitmap _imageSource;
        public Bitmap ImageSource
        {
            get => _imageSource;
            set
            {
                this.RaiseAndSetIfChanged(ref _imageSource,value);
            }
        }

        public ReactiveCommand<Unit, Unit> CreateRoomCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> JoinRoomCommand { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
