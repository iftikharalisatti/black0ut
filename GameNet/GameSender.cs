using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace Black0ut.GameNet
{
    public class GameSender : IPEndPoint
    {
        public object Storage;
        public long LastReceiveTime;

        public GameSender(IPEndPoint endPoint, long lastReceiveTime) : base(endPoint.Address, endPoint.Port)
        {
            LastReceiveTime = lastReceiveTime;
        }
    }
}
