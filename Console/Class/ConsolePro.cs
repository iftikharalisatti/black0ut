// --------------------------------
// Author - Black0uter (Nick)
//
// Copyright (C) 2018
// GNU GENERAL PUBLIC LICENSE V3
// --------------------------------

using System;
using System.Threading;
using System.Collections.Generic;

namespace Black0ut.Console
{
    using Log;

    public class ConsolePro
    {
        public Func<string> ConsoleReadFunc;

        public Dictionary<string, Action<string>> ConsoleActions;

        public Thread ConsoleThread;

        public ConsolePro(Func<string> consoleReadFunc, params ConsoleAction[] consoleActions)
        {
            ConsoleReadFunc = consoleReadFunc;

            ConsoleActions = new Dictionary<string, Action<string>>();

            // ADD ACTIONS
            for (int i = 0, count = consoleActions.Length; i < count; i++)
            {
                var action = consoleActions[i];

                ConsoleActions.Add(action.Comand, action.Action);
            }

            ConsoleThread = new Thread(() =>
            {
                Log.Show("Thread started.", "ConsoleThread");

                while (true)
                {
                    try
                    {
                        var input = ConsoleReadFunc();

                        var comand = input.Split(' ')[0];

                        if (!ConsoleActions.ContainsKey(comand))
                        {
                            Log.Show($"Ho handlers for comand [{comand}].", "ConsoleThread");
                            continue;
                        }

                        ConsoleActions[comand]?.Invoke(input);
                    }
                    catch (Exception e)
                    {
                        Log.Show($"Exception [{e}].", "ConsoleThread");
                    }
                }
            });

            ConsoleThread.Start();

            Log.Show("Instance created.", "ConsolePro");
        }
    }
}
