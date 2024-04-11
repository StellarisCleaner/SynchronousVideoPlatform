using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using LibVLCSharp.Shared;
using Newtonsoft.Json.Linq;
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
using ViedoAsyncAvalonia.Services;

namespace ViedoAsyncAvalonia.ViewModels
{
    public class VideoViewModel : ViewModelBase
    {
        LibVLC libVLC;

        public event Action<string> CommandReceived;
        private Uri _videoUrl;
        public Uri VideoUrl
        {
            get => _videoUrl;
            set => this.RaiseAndSetIfChanged(ref _videoUrl, value);
        }
        public bool IsLive { get => _isLive;
            set {
                _isLive = value;
                OnPropertyChanged(nameof(IsLive));
            }
        }
        private bool _isLive = true;
        
        public double CurrentPosition
        {
            get
            {
                return _currentPosition;
            }
            set
            {
                Debug.WriteLine($"Attempting to set CurrentPosition from {_currentPosition} to {value}");
                if (_currentPosition != value)
                {
                    this.RaiseAndSetIfChanged(ref _currentPosition,value);
                    if (!_updatingFromMediaPlayer&&MediaPlayer.Media!=null)
                    {
                        MediaPlayer.Position = (float)value;
                    }
                    
                }
            }
        }
        private double _currentPosition;
        private string _currentPositionString;
        public ReactiveCommand<Unit, Unit> PlayCommand { get; }
        public ReactiveCommand<Unit, Unit> PauseCommand { get; }
        public ReactiveCommand<Unit, Unit> SkipCommand { get; }
        public ReactiveCommand<Unit, Unit> BackCommand { get; }
        public ReactiveCommand<Unit, Unit> OpenFileCommand { get; }

        private MediaPlayer _mediaPlayer;
        private bool _updatingFromMediaPlayer;

        public event Action<string> RequestSend;

        public void OnRequestSend(string message)
        {
            RequestSend?.Invoke(message);
        }
        public Func<string,Task> SendMsgToServerAction { get; set; }
        public string TempValue = "0";
        private readonly FilesService _filesService;
        public VideoViewModel(App app)
        {
            //_filesService = filesService;
            // 初始化LibVLC和MediaPlayer
            Application? current = Application.Current;
            libVLC = new LibVLC();
            _filesService = app.FilesService;
            PlayCommand = ReactiveCommand.Create(PlayVideo);
            PauseCommand = ReactiveCommand.Create(PauseVideo);
            SkipCommand = ReactiveCommand.Create(SkipVideo);
            BackCommand = ReactiveCommand.Create(BackVideo);
            OpenFileCommand = ReactiveCommand.CreateFromTask(OpenFileDialogAsync);
            // 示例视频URL
        }

        private async Task OpenFileDialogAsync()
        {


            //Avalonia.Platform.Storage.IStorageFile? storageFile = await _filesService.OpenFileAsync();
            //string name = storageFile.Name;
            //if (file != null)
            //{
            //    Debug.WriteLine(file.Name);
            //}
        }

        private void BackVideo()
        {
            if (MediaPlayer != null)
            {
                MediaPlayer.Position -= 0.05f;

                ///判断是否为房主并向服务器发送消息
                if (true)
                {

                }
            }
        }

        private void SkipVideo()
        {
            if (MediaPlayer != null)
            {
                MediaPlayer.Position += 0.05f;
            }
        }
        public void SendMsgToServer(string msg)
        {
            
        }





        private void PlayVideo()
        {
            if (MediaPlayer == null)
            {
                MediaPlayer = new MediaPlayer(libVLC);
                MediaPlayer.PositionChanged += (object sender, MediaPlayerPositionChangedEventArgs e) =>
                {
                    _updatingFromMediaPlayer = true;
                    CurrentPosition = e.Position;
                    _updatingFromMediaPlayer = false;
                };
            }
            if (MediaPlayer != null)
            {
                var mediaPath = @"C:\Coding\AsyncPlayer\ViedoAsyncAvalonia\ViedoAsyncAvalonia\Assets\testVideo.mp4";
                //var mediaPath = @"https://xy1x69x96x236xy.mcdn.bilivideo.cn:8082/v1/resource/1472300630-1-30112.m4s?agrr=0&build=0&buvid=9BD807B2-5CE5-2069-6F1B-F7492B61671D67658infoc&bvc=vod&bw=310840&deadline=1712421853&e=ig8euxZM2rNcNbdlhoNvNC8BqJIzNbfqXBvEqxTEto8BTrNvN0GvT90W5JZMkX_YN0MvXg8gNEV4NC8xNEV4N03eN0B5tZlqNxTEto8BTrNvNeZVuJ10Kj_g2UB02J0mN0B5tZlqNCNEto8BTrNvNC7MTX502C8f2jmMQJ6mqF2fka1mqx6gqj0eN0B599M%3D&f=u_0_0&gen=playurlv2&logo=A0020000&mcdnid=50000516&mid=1752496&nbs=1&nettype=0&oi=1942389617&orderid=0%2C3&os=mcdn&platform=pc&sign=d73d2d&traceid=trHRJscfCnqutg_0_e_N&uipk=5&uparams=e%2Cuipk%2Cnbs%2Cdeadline%2Cgen%2Cos%2Coi%2Ctrid%2Cmid%2Cplatform&upsig=188b94bf979613004bc5e11193179b15";
                var formattedPath = $"file:///{mediaPath.Replace("\\", "/")}";
                //VideoUrl = formattedPath;
                //VideoUrl = mediaPath;
                var media = new Media(libVLC, formattedPath, FromType.FromLocation);
                MediaPlayer.Media = media;
                MediaPlayer.Play();
            }
               
                
                
        }


        private void PauseVideo()
        {
            MediaPlayer.Pause();
        }

        // 确保释放资源
        public void Dispose()
        {
            _mediaPlayer.Dispose();
        }

        internal void StopVideo()
        {
            if (MediaPlayer != null)
            {
                MediaPlayer.Stop();
            }
        }

        public event Action<MediaPlayer> MediaPlayerChanged;

        public MediaPlayer MediaPlayer
        {
            get => _mediaPlayer;
            private set
            {
                this.RaiseAndSetIfChanged(ref _mediaPlayer, value);
                //if (_mediaPlayer != value)
                //{
                //    _mediaPlayer = value;
                //    MediaPlayerChanged?.Invoke(_mediaPlayer);
                //}
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }




    }
}
