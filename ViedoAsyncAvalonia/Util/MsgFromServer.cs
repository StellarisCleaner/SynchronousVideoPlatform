using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViedoAsyncAvalonia.Util
{
    public class MsgFromServer
    {
        public bool success = false;
        public string error = string.Empty;
        public string uuid = string.Empty;
        public UserForJson user;
    }
}
