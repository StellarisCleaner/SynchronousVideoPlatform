using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using LibVLCSharp.Shared;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace ViedoAsyncAvalonia.Util
{
    public class NetworkUtil
    {
        public bool IsLogined = false;
        static ClientWebSocket clientWebSocket;
        public event Action<string> CommandReceived;
        public event Action WebSocketClose;
        public event Action<User> Loginsuccess;
        public event Action<string> CommandSend;
        public event Action AsyncPositionRequest;
        public event Action<float> AsyncPosition;
        public event Action IntoRoom;
        public event Action LeaveRoom;
        public event Action SomeOneIntoRoom;
        public event Action SomeOneLeftRoom;
        public event Action MasterLeftRoom;
        public event Action<List<RoomForJson>> GetAllRooms;
        public HttpClient Client;
        private object _lock = new object();
        private static readonly NetworkUtil instance = new NetworkUtil();
        public static NetworkUtil Instance
        {
            get
            {
                return instance;
            }
        }



        // 调用这个方法来触发事件
        protected virtual void OnCommandReceived(string command)
        {
            CommandReceived?.Invoke(command);
        }
        /// <summary>
        /// 有user加入房间
        /// </summary>
        /// <param name="command"></param>
        protected virtual void OnUserJoinRoom()
        {
            SomeOneIntoRoom?.Invoke();
        }
        protected virtual void OnWebSocketClose()
        {
            WebSocketClose?.Invoke();
        }protected virtual void OnLoginsuccess(User localuser)
        {
            Loginsuccess?.Invoke(localuser);
            IsLogined = true;
        }protected virtual void OnAsyncPositionRequest()
        {
            AsyncPositionRequest?.Invoke();
        }protected virtual void OnAsyncPosition(float Videoposition)
        {
            AsyncPosition?.Invoke(Videoposition);
        }protected virtual void OnIntoRoom()
        {
            IntoRoom?.Invoke();
        }protected virtual void OnLeaveRoom()
        {
            LeaveRoom?.Invoke();
        }
        protected virtual void PreCommandSend(string command)
        {
            CommandSend?.Invoke(command);
        }
        public async Task ConnToServer(string ipaddress,string port)
        {
            Uri serverUri = new Uri($"ws://{ipaddress}:{port}"); // 你要连接的 WebSocket 服务器地址
            
            clientWebSocket = new ClientWebSocket();
            
            try
            {
                await clientWebSocket.ConnectAsync(serverUri, CancellationToken.None);

                await Receive();

                await Send("连接断开");

                OnWebSocketClose();

                await clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception: {ex}");
            }
        }
       
        public async Task Send(string message)
        {
            if (clientWebSocket == null)
            {
                return;
            }
            if (clientWebSocket.State == WebSocketState.Open)
            {
                byte[] buffer = Encoding.UTF8.GetBytes(message);
                await clientWebSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
                return;
            }
            return;
        }
        
        async Task Receive()
        {
            byte[] buffer = new byte[1024];
            while (clientWebSocket.State == WebSocketState.Open)
            {
                var result = await clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                string message = Encoding.UTF8.GetString(buffer, 0, result.Count);

                Debug.WriteLine($"Received: " + message);

                MessageFormServerAsync(message);
            }
            Debug.WriteLine("不会吧 不会吧");
        }
        
        private async Task MessageFormServerAsync(string message)
        {
            OnCommandReceived(message);
            string[] mess = message.Split(" ");
            switch (mess[0])
            {
                case "LoginReturn":
                    {
                        MsgFromServer msgFromServer = JsonConvert.DeserializeObject<MsgFromServer>(mess[1]);

                        if (msgFromServer.success)
                        {
                            User user = new User(msgFromServer.user.uuid, msgFromServer.user.username, msgFromServer.user.avatar_url);
                            if (user != null)
                            {
                                user = await user.SetAvatar(user.avatar_url);
                                if (user != null)
                                    OnLoginsuccess(user);
                            }
                            
                            //user.Avatar.Save("?????.png");
                            
                           
                        }
                        break;
                    }
                case "GetAllRooms":
                    {
                        Debug.WriteLine("为什么要演奏春日影");
                        List<RoomForJson> rooms = JsonConvert.DeserializeObject<List<RoomForJson>>(mess[1]);
                        Debug.WriteLine($"{rooms[0].note}");
                        OnGetAllRooms(rooms);
                        break;
                    }
                case "heartbeat":
                    {
                        
                        break;
                    }
                case "AsyncPositionRequest":
                    {
                        OnAsyncPositionRequest();
                        break;
                    }
                case "UserJoinRoom":
                    {
                        UserForJson? userForJson = JsonConvert.DeserializeObject<UserForJson>(mess[1]);
                        User user = new User(userForJson);
                        user = await user.SetAvatar(user.avatar_url);
                        OnUserJoinRoom();
                        break;
                    }
                case "AsyncPosition":
                    {
                        try
                        {
                            float position = float.Parse(mess[1]);
                            OnAsyncPosition(position);
                        }catch (Exception ex) 
                        {
                            Debug.WriteLine(ex.ToString()+"肯定是参数有问题" + mess[1]);
                        }
                        
                        break;
                    }
                case "加入房间成功":
                    {
                        OnIntoRoom();
                        break;
                    }
                case "创建房间成功":
                    {
                        //房间信息获取
                        //MainController.VideoFormDisplay();
                        OnIntoRoom();
                        break;
                    }
                case "离开房间成功":
                    {
                        OnLeaveRoom();
                        break;
                    }
                default: { break; }
            }
        }

        private void OnGetAllRooms(List<RoomForJson>? rooms)
        {
            GetAllRooms.Invoke(rooms);
        }

        public NetworkUtil()
        {
            if (clientWebSocket == null)
            {
                Client = new HttpClient();
            }
        }
        public async Task<HttpResponseMessage> UploadImage(string imagePath, string uploadUrl,string uuid)
        {
            using MultipartFormDataContent content = new MultipartFormDataContent();
            using FileStream fileStream = File.OpenRead(imagePath);
            content.Add(new StreamContent(fileStream), "image", Path.GetFileName(imagePath));

            // 构建带有查询参数的URL
            UriBuilder uriBuilder = new UriBuilder(uploadUrl);
            System.Collections.Specialized.NameValueCollection query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query["uuid"] = uuid;
            uriBuilder.Query = query.ToString();
            string urlWithQuery = uriBuilder.ToString();

            HttpResponseMessage response = await Client.PostAsync(urlWithQuery, content);
            //response.EnsureSuccessStatusCode();
            return response;
        }
        public void SaveBitmap(Bitmap bitmap, string fileName)
        {
            // 获取当前程序的路径
            string currentDirectory = Directory.GetCurrentDirectory();

            // 创建 /avatar/ 文件夹的路径
            string directoryPath = Path.Combine(currentDirectory, "avatar");

            // 如果 /avatar/ 文件夹不存在，就创建一个新的文件夹
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            // 创建图片文件的路径
            string filePath = Path.Combine(directoryPath, fileName);

            // 检查是否存在一个文件名的前36位与 fileName 相同的文件
            var existingFile = Directory.GetFiles(directoryPath)
                .FirstOrDefault(file => Path.GetFileName(file).Length >= 36 &&
                                        Path.GetFileName(file).Substring(0, 36) == fileName.Substring(0, 36));

            if (existingFile != null)
            {
                string existingTimestampStr = Path.GetFileNameWithoutExtension(existingFile).Split("TIME").Last();
                string newTimestampStr = fileName.Split("TIME")[1].Split(".")[0];
                // 提取两个文件名中 "TIME" 后面的时间戳
                long existingTimestamp = long.Parse(existingTimestampStr);
                long newTimestamp = long.Parse(newTimestampStr);


                // 如果新的时间戳不大于已存在的时间戳，就不保存图片
                if (newTimestamp <= existingTimestamp)
                {
                    return;
                }
            }

            // 保存图片
            bitmap.Save(filePath);
        }
        public async Task<Bitmap> GetAvatarAsync(string imgName)
        {
            try
            {
                ///处理url 加上数据库服务器信息
                string v = "http://127.0.0.1:17905/images/" + imgName;
                Bitmap bitmap = await SetAvatar(v);
                SaveBitmap(bitmap,imgName);
                return bitmap;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                Debug.WriteLine("我焯 没用");
                return null;
            }
            
        }
        public async Task<Avalonia.Media.Imaging.Bitmap> SetAvatar(string Imgurl)
        {
            byte[] bytes = await NetworkUtil.instance.Client.GetByteArrayAsync(Imgurl);
            if (bytes != null)
            {
                using var stream = new MemoryStream(bytes);
                return new Avalonia.Media.Imaging.Bitmap(stream);
            }
            return null;
            

        }

    }
}
