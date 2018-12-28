using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace Black0ut.GameNet
{
    public class GamePacket
    {
        public byte[] Data;
        public long ReceiveTime;
        public IPEndPoint Sender;
    }
}
