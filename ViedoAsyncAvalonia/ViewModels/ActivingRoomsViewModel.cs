using Newtonsoft.Json;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using ViedoAsyncAvalonia.Util;
using ViedoAsyncAvalonia.Views;
using static ViedoAsyncAvalonia.Util.NetworkUtil;

namespace ViedoAsyncAvalonia.ViewModels
{
    public class ActivingRoomsViewModel:ViewModelBase
    {
        public bool IsOverlayVisible
        {
            get=>_isOverlayVisible; set=>this.RaiseAndSetIfChanged(ref _isOverlayVisible,value);
        }
        private bool _isOverlayVisible =  true;
        public ObservableCollection<Room> Rooms { get; } = new ObservableCollection<Room>();
        public ReactiveCommand<string, Unit> BorderCommand { get; }
        public ReactiveCommand<Unit, Unit> CreateRoomCommand { get; }
        public ActivingRoomsViewModel()
        {
            NetworkUtil.Instance.IntoRoom += OnIntoRoom;
            NetworkUtil.Instance.LeaveRoom += OnLeaveRoom;
            NetworkUtil.Instance.GetAllRooms += OnGetAllRooms;
            NetworkUtil.Instance.Loginsuccess += OnLoginsuccess;
            // 示例：添加几个房间到集合中，实际应用中你可能会从服务器获取这些数据
            BorderCommand = ReactiveCommand.Create<string>(OnRoomBorderClicked);
            CreateRoomCommand = ReactiveCommand.Create(OnCreateRoomCommand);
            // 添加更多房间...
            if (NetworkUtil.Instance.IsLogined)
            {
                NetworkUtil.Instance.Send("GetAllRooms");
            }


            
        }

        private void OnLoginsuccess(User obj)
        {
            IsOverlayVisible = false;
        }

        private void OnLeaveRoom()
        {
            IsOverlayVisible = false;
        }

        private void OnGetAllRooms(List<RoomForJson> rooms)
        {
            foreach (RoomForJson room in rooms) 
            {
                Rooms.Add(new Room(room.master,room.roomId,room.clients,room.note));
            }
        }

        private void OnIntoRoom()
        {
            IsOverlayVisible = true;
        }

        private void OnCreateRoomCommand()
        {
            CreateRoomWindow createRoomWindow = new CreateRoomWindow();
            createRoomWindow.Show();
        }

        public bool IsJoining
        {
            get => _isJoining;
            set => this.RaiseAndSetIfChanged(ref _isJoining,value);
        }
        private bool _isJoining = false;
        private void OnRoomBorderClicked(string roomId)
        {
            // 在这里处理你的命令，你已经获取到了RoomId
            Console.WriteLine(roomId);
        }


    }
}
