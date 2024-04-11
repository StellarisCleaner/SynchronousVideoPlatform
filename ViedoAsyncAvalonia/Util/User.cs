using Avalonia.Media.Imaging;
using Avalonia.Platform;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViedoAsyncAvalonia.ViewModels;

namespace ViedoAsyncAvalonia.Util
{
    public class User:ViewModelBase
    {
        public string username;
        public string uuid;
        public string avatar_url;
        public Bitmap Avatar { get => _avatar; set => this.RaiseAndSetIfChanged(ref _avatar, value); }
        private Bitmap _avatar;
        public static int temp = 0;

        public User()
        {
            //AvatarInitialized();
        }

        private void AvatarInitialized()
        {
            string v = "avares://ViedoAsyncAvalonia/Assets/DefalutProfile.png";
            Avatar = new Bitmap(AssetLoader.Open(new Uri(v)));
        }

        public User(string uuid,string username,string avatar_url)
        {
            this.uuid = uuid;
            this.username = username;
            this.avatar_url = avatar_url;
            AvatarInitialized();
            
        }
        public User(UserForJson user)
        {
            this.uuid = user.uuid;
            this.username = user.username;
            if (user.avatar_url != null)
            {
                this.avatar_url = user.avatar_url;
            }
            
            AvatarInitialized();
            
        }

        public async Task<User> SetAvatar(string avatar_url)
        {
            if (avatar_url != null)
            {
                Bitmap bitmap = await NetworkUtil.Instance.GetAvatarAsync(avatar_url);
                Avatar = bitmap;
                return this;
            }
            return this;
        }
    }
}
