// --------------------------------
// Author - Black0uter (Nick)
//
// Copyright (C) 2018
// GNU GENERAL PUBLIC LICENSE V3
// --------------------------------

using System.Net;

namespace Black0ut.Net
{
    public interface IDatagram
    {
        IPEndPoint IPEndPoint { get; set; }
        byte[] Data { get; set; }
    }
}