using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using ViedoAsyncAvalonia.Util;
using ViedoAsyncAvalonia.Views;

namespace ViedoAsyncAvalonia.ViewModels
{
    public class CreateRoomViewModel : ViewModelBase
    {
        public string Note
        {
            get => _note;
            set { this.RaiseAndSetIfChanged(ref _note, value); }
        }
        private string _note = "测试用例";
        public ReactiveCommand<Unit, Unit> CreateRoomCommand { get; }
        public CreateRoomViewModel(){
            CreateRoomCommand = ReactiveCommand.Create(OnCreateRoomCommand);
            NetworkUtil.Instance.CommandReceived += CommandRecive;
        }

        private void CommandRecive(string msg)
        {
            if(msg.StartsWith("创建房间成功"))
            new TipWindow(msg).Show();
        }

        private void OnCreateRoomCommand()
        {
            NetworkUtil.Instance.Send($"CreateRoom {Note}");
        }
        
    }
}
