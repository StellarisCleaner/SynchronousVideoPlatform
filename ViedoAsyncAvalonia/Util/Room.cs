using Avalonia.Input;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ViedoAsyncAvalonia.ViewModels;

namespace ViedoAsyncAvalonia.Util
{
    public class Room: ViewModelBase
    {
        public string RoomId { get
            {
                return _roomId;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref _roomId, value);
            }
        }
        public string _roomId;

        public string Note { get=>_name; set
            {
                this.RaiseAndSetIfChanged(ref _name, value);
            }
        }
        public int ClientNumber { get=>_clientNumber; set=>this.RaiseAndSetIfChanged(ref _clientNumber,value); }
        public int _clientNumber;
        public string _name;
        public string MasterName { get=>_masterID; set
            {
                this.RaiseAndSetIfChanged(ref _masterID,value);
            }
        }
        public string _masterID;
        public string Img { get; set; }
        /// <summary>
        ///下面这个bool来判断是否从服务器上获得成功等信息，然后当循环时间到达指定时间，置true
        /// </summary>
        private bool _locked = false;



        public Room(string master,int roomId, int clientNumber,string note)
        {
            MasterName = master;
            RoomId = roomId.ToString();
            ClientNumber = clientNumber;
            Note = note;
        }

        public async void ClickOn()
        {
            await NetworkUtil.Instance.Send($"JoinRoom {RoomId}");
            
        }
    }
}
