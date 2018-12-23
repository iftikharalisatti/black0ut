// --------------------------------
// Author - Black0uter (Nick)
//
// Copyright (C) 2018
// GNU GENERAL PUBLIC LICENSE V3
// --------------------------------

using System.Net;

namespace Black0ut.Net
{
    public struct IPEndPointTimePair
    {
        public IPEndPoint IPEndPoint
        {
            get
            {
                return iPEndPoint;
            }
            set
            {
                iPEndPoint = value;
            }
        }
        public IPEndPoint iPEndPoint;

        public int Time
        {
            get
            {
                return time;
            }
            set
            {
                time = value;
            }
        }
        public int time;

        #region .ctor

        public IPEndPointTimePair(IPEndPoint _iPEndPoint, int _time)
        {
            iPEndPoint = _iPEndPoint;
            time = _time;
        }

        #endregion

        #region Inherit

        public override bool Equals(object obj)
        {
            return iPEndPoint.Equals(((IPEndPointTimePair)obj).IPEndPoint);
        }

        public override int GetHashCode()
        {
            return iPEndPoint.GetHashCode();
        }

        public override string ToString()
        {
            return $"IPEndPoint: {iPEndPoint}";
        }

        #endregion
    }
}
