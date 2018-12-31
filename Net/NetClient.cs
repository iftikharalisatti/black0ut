// --------------------------------
// Author - Black0uter (Nick)
//
// Copyright (C) 2018
// GNU GENERAL PUBLIC LICENSE V3
// --------------------------------

using System.Net;

namespace Black0ut.Net
{
    public class NetClient<TCustomData> : IPEndPoint
    {
        public TCustomData CustomData;

        public long LastReceiveTime;

        public NetClient(IPEndPoint endPoint, long lastReceiveTime) : base(endPoint.Address, endPoint.Port)
        {
            LastReceiveTime = lastReceiveTime;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public override bool Equals(object comparand)
        {
            return base.Equals(comparand);
        }
    }
}
