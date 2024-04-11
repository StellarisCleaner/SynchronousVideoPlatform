using Avalonia.Markup.Xaml.Templates;
using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViedoAsyncAvalonia.ViewModels
{
    public class MenuItemViewModel:ViewModelBase
    {
        public string Name { get; set; }
        public ViewModelBase ViewModel { get; set; }
        public Bitmap ImageSource { get; set; } // 用户头像URL
        public bool IsUserAvatar { get; set; } = false; // 是否为用户头像项
        
    }
}
