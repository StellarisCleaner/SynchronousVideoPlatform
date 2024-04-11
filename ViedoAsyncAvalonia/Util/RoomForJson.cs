using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViedoAsyncAvalonia.Util
{
    public class RoomForJson
    {
        //[{"roomId":"0","master":"野兽先辈","clients":0,"note":"测试用例"}]
        public int roomId = 0;
        public int clients = 0;
        public string note = string.Empty;
        public string master = string.Empty;
    }
}
