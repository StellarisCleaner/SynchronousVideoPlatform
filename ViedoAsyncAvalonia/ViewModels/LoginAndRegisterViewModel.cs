using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ReactiveUI;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ViedoAsyncAvalonia.Util;
using ViedoAsyncAvalonia.Views;

namespace ViedoAsyncAvalonia.ViewModels
{
    public class LoginAndRegisterViewModel:ViewModelBase
    {
        
        private string _username;
        public string username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged(nameof(username));

            }
        }
        private string _password;
        public string password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged(nameof(password));
            }
        }
        public Bitmap AvatarImageSource
        {
            get => _avatar;
            set
            {
                this.RaiseAndSetIfChanged(ref _avatar, value);
                Debug.WriteLine("头像改变成功");
            }
        }
        private Bitmap _avatar;
        private string _registerUsername;
        public string RegisterUsername
        {
            get => _registerUsername;
            set
            {
                this.RaiseAndSetIfChanged(ref _registerUsername,value);
            }
        }
        private string _registerpassword;
        public string Registerpassword
        {
            get => _registerpassword;
            set
            {
                this.RaiseAndSetIfChanged(ref _registerpassword, value);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public ReactiveCommand<Unit, Unit> RegisterRequest { get; }
        public ReactiveCommand<Unit, Unit> LoginRequest { get; }
        public ReactiveCommand<Unit, Unit> AnonymousLoginRequest { get; }

        public LoginAndRegisterViewModel()
        {
            NetworkUtil.Instance.CommandReceived += LoginReturn;
            RegisterRequest = ReactiveCommand.Create(RegisterAsync);
            LoginRequest = ReactiveCommand.Create(LoginAsync);
            AnonymousLoginRequest = ReactiveCommand.Create(AnonymousLogin);
            
        }

        private async void  AnonymousLogin()
        {
            await NetworkUtil.Instance.Send($"anonymouslogin");
        }

        private async void LoginAsync()
        {
            string pattern = @"^[^\p{P}]{1,15}$";//小于15个字符并且不包含标点符号
            if (Regex.IsMatch(username, pattern))
            {
                await NetworkUtil.Instance.Send($"login {username} {password}");
            }
            else
            {
                Debug.WriteLine("你这不行呀");
            }
        }

        private async void RegisterAsync()
        {
            ///检测输入的格式
            string pattern = @"^[^\p{P}]{1,15}$";//小于15个字符并且不包含标点符号
            if (Regex.IsMatch(RegisterUsername, pattern))
            {
                await NetworkUtil.Instance.Send($"register {RegisterUsername} {Registerpassword}");
            }
            else
            {
                Debug.WriteLine("你这不行呀");
            }

        }
        private async void LoginReturn(string command)
        {
            string[] mess = command.Split(" ");
            string prefix = mess[0];
            switch (prefix)
            {
                case "LoginReturn":
                    {

                        MsgFromServer? msgFromServer = JsonConvert.DeserializeObject<MsgFromServer>(command.Substring(prefix.Length));


                        if (msgFromServer.success)
                        {
                            ///去获取头像
                            ///
                            //NetworkUtil.Instance.GetAvatarAsync(null);

                            new TipWindow("登录成功").Show();
                        }
                        else
                        {
                            Debug.WriteLine($"{msgFromServer.error}");
                            new TipWindow("登录失败"+msgFromServer.error.ToString()).Show();
                        }
                        break;
                    }
                case "RigisterReturn":
                    {
                        MsgFromServer? msgFromServer = JsonConvert.DeserializeObject<MsgFromServer>(command.Substring(prefix.Length));
                        if (msgFromServer.success)
                        {
                            new TipWindow("注册成功 请登录").Show();
                            if (AvatarImageSource != null)
                            {
                               // string Avatarbase64 = UploadAvatar(AvatarImageSource);
                                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "avatar");
                                string imgpath = GetFilePath(path,"Temp");
                                System.Net.Http.HttpResponseMessage httpResponseMessage = await NetworkUtil.Instance.UploadImage(imgpath, "http://127.0.0.1:17905/upload-image",msgFromServer.uuid);
                                httpResponseMessage.Content.ToString();
                            }

                            return;
                        }
                        else
                        {
                            new TipWindow("注册失败 看看是啥原因" + msgFromServer.error.ToString()).Show();
                        }
                        break;
                    }
                default: { break; }
            }
        }

        private string UploadAvatar(Bitmap bitmap)
        {
            using (var stream = new MemoryStream())
            {
                bitmap.Save(stream);
                var bytes = stream.ToArray();
                return Convert.ToBase64String(bytes);
            }
        }

        internal void SetSelfAvatar(Uri path)
        {

            AvatarImageSource = new Bitmap(path.LocalPath);

            // 获取图片的后缀名
            var extension = Path.GetExtension(path.LocalPath);

            // 构造目标路径
            var destinationPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "avatar", "Temp" + extension);

            // 确保目标目录存在
            Directory.CreateDirectory(Path.GetDirectoryName(destinationPath));

            // 将图片复制到目标路径
            File.Copy(path.LocalPath, destinationPath, true);
        }
        public string GetFilePath(string directoryPath, string fileNameWithoutExtension)
        {
            var files = Directory.GetFiles(directoryPath, fileNameWithoutExtension + ".*");
            if (files.Length > 0)
            {
                return files[0];
            }
            else
            {
                return null;
            }
        }
    }
}
