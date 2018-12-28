// --------------------------------
// Author - Black0uter (Nick)
//
// Copyright (C) 2018
// GNU GENERAL PUBLIC LICENSE V3
// --------------------------------

using System;

namespace Black0ut.Log
{
    public static class Log
    {
        public static Action<string, string> LogAction;

        public static void Show(string message, string method)
        {
            LogAction?.Invoke(message, method);
        }
    }
}