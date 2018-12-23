// --------------------------------
// Author - Black0uter (Nick)
//
// Copyright (C) 2018
// GNU GENERAL PUBLIC LICENSE V3
// --------------------------------

using System;

namespace Black0ut.Net
{
    public class DataHandler
    {
        public DDataHandler[] DataHandlers;

        public void Append(DDataHandler[] dataHandlers)
        {
            var newDataHandlers = new DDataHandler[DataHandlers.Length];

            var index = 0;

            // APPEND OLD HANDLERS
            for (int i = 0, length = DataHandlers.Length; i < length; i++)
            {
                newDataHandlers[index++] = DataHandlers[i];
            }

            // APPEND NEW HANDLERS
            for (int i = 0, length = dataHandlers.Length; i < length; i++)
            {
                newDataHandlers[index++] = dataHandlers[i];
            }

            DataHandlers = newDataHandlers;
        }

        public bool Handle(Datagram datagram)
        {
            if (datagram.Data[0] >= DataHandlers.Length)
                throw new Exception($"No DataHandlers for '{datagram.Data[0]}' data type!");

            return DataHandlers[datagram.Data[0]](datagram);
        }
    }
}