// --------------------------------
// Author - Black0uter (Nick)
//
// Copyright (C) 2018
// GNU GENERAL PUBLIC LICENSE V3
// --------------------------------

using System;
using System.Net;
using Black0ut.Byte;

namespace Black0ut.Net
{
    public struct Datagram : IDatagram
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

        public byte[] Data
        {
            get
            {
                return data;
            }
            set
            {
                data = value;
            }
        }
        public byte[] data;

        #region .ctor

        public Datagram(IPEndPoint _iPEndPoint, byte[] _data)
        {
            iPEndPoint = _iPEndPoint;
            data = _data;
        }

        #endregion

        #region Inherit

        public override bool Equals(object obj)
        {
            var datagram = (Datagram)obj;

            if (!iPEndPoint.Equals(datagram.IPEndPoint))
                return false;

            return ProByte.Equals(data, datagram.data);
        }

        public override int GetHashCode()
        {
            return data.Length ^ data[0] ^ data[Data.Length - 1] ^ iPEndPoint.GetHashCode();
        }

        public override string ToString()
        {
            return $"IPEndPoint: {iPEndPoint}, Data: {BitConverter.ToString(data)}";
        }

        #endregion
    }
}