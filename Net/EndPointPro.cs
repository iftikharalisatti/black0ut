// --------------------------------
// Author - Black0uter (Nick)
//
// Copyright (C) 2018
// GNU GENERAL PUBLIC LICENSE V3
// --------------------------------

using System.Net;

namespace Black0ut.Net
{
    using Byte;

    public static class EndPointPro
    {
        public static bool Equals(IPEndPoint a, IPEndPoint b)
        {
            return a.Port == b.Port && BytePro.Equals(a.Address.GetAddressBytes(), b.Address.GetAddressBytes());
        }
    }
}
