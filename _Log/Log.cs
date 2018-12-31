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
        public static Action<string> LogAction;

        public static Action<string, string> LogActionWithMethod;

        /// <summary>
        /// Standart log. Invokes LogAction.
        /// </summary>
        public static void Show(string message)
        {
            LogAction?.Invoke(message);
        }

        /// <summary>
        /// Log with method trace. Invokes LogActionWithMethod.
        /// </summary>
        public static void Show(string message, string method)
        {
            LogActionWithMethod?.Invoke(message, method);
        }
    }
}