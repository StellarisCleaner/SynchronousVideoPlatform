using Newtonsoft.Json;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tmds.DBus.Protocol;
using ViedoAsyncAvalonia.Util;

namespace ViedoAsyncAvalonia.ViewModels
{
    public class RoomInWindowViewModel:ViewModelBase
    {
        
        public List<User> Users { get =>_users; set=>this.RaiseAndSetIfChanged(ref _users,value); }
        private List<User> _users = new List<User>();
        public User Master { get => _master; set => this.RaiseAndSetIfChanged(ref _master, value); }
        public User _master;
        public String Status {
            get =>_status;
            set => this.RaiseAndSetIfChanged(ref _status, value);
        }
        private String _status;
        public RoomInWindowViewModel()
        {
            NetworkUtil.Instance.CommandReceived += RoomInfoRecive;
            NetworkUtil.Instance.LeaveRoom+= LeaveAndClose;
        }

        private void LeaveAndClose()
        {
            Users.Clear();
        }

        private async void RoomInfoRecive(string msg)
        {
            InitializeRoomInfo();

            string cmd = msg.Split(" ")[0];
            if (cmd == "UserInfo")
            {
                ///预期是this.send("UserInfo Users "+JSON.stringify(room.clients)+
                ///                                            "MasterInfo "+JSON.stringify(room.master));
                string userJson = msg.Split("Users ")[1].Split("MasterInfo ")[0];
                UserForJson[]? usersForJson = JsonConvert.DeserializeObject<UserForJson[]>(userJson);
                List<User> users = new List<User>();
                usersForJson.ToList().ForEach(async user =>
                {
                    User usertemp = new User(user.uuid, user.username, user.avatar_url);
                    //设置头像
                    usertemp =await usertemp.SetAvatar(user.avatar_url);
                    users.Add(usertemp);
                });
                Users = users;
                string masterInfoJson = msg.Split("MasterInfo ")[1];
                UserForJson? master = JsonConvert.DeserializeObject<UserForJson>(masterInfoJson);
                User masterTemp = new User(master.uuid, master.username, master.avatar_url);
                //设置房主的头像
                masterTemp = await masterTemp.SetAvatar(master.avatar_url);
                Master = masterTemp;
            }
            
        }

        private void InitializeRoomInfo()
        {
            this.Users.Clear();
            Master = null;
        }
    }
}
