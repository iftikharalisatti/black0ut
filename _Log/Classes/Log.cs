// --------------------------------
// Author - Black0uter (Nick)
//
// Copyright (C) 2018
// GNU GENERAL PUBLIC LICENSE V3
// --------------------------------

using System;

namespace Black0ut.Log
{
    public class Log
    {
        public Action<string, string> LogAction;

        public Log(Action<string, string> logAction)
        {
            LogAction = logAction;
        }

        public void Show(string message, string method)
        {
            LogAction(message, method);
        }
    }
}