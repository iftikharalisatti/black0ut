// --------------------------------
// Author - Black0uter (Nick)
//
// Copyright (C) 2018
// GNU GENERAL PUBLIC LICENSE V3
// --------------------------------

using System;
using System.Net;
using System.Threading;
using System.Collections.Generic;

namespace Black0ut.Net
{
    using Log;
    using Byte;

    public class P2PClient : QueuedUDPClient
    {
        public int IPEndPointTimeout;

        public List<IPEndPointTimePair> IPEndPoints;

        public Thread IPEndPointsUpdateThread;

        public int Time;

        public P2PClient(string hostname, int port, Log log, int dequeueLoopDelay = 10, int iPEndPointTimeout = 1000) : base(hostname, port, log, dequeueLoopDelay)
        {
            IPEndPoints = new List<IPEndPointTimePair>();

            DataHandler.Append(new DDataHandler[]
            {
                _IPEndPoints, AddIPEndPoint, GetIPEndPoints
            });

            IPEndPointsUpdateThread = new Thread(() =>
            {
                Log.Show($"Started thread TimeThread.", "IPEndPointsUpdateThread");

                var iPEndPointTimePair = new IPEndPointTimePair();

                while (true)
                {
                    for (int i = 0; i < IPEndPoints.Count; i++)
                    {
                        iPEndPointTimePair = IPEndPoints[i];

                        if (Time - iPEndPointTimePair.Time > IPEndPointTimeout)
                            IPEndPoints.Remove(iPEndPointTimePair);

                        Interlocked.Increment(ref Time);

                        Thread.Sleep(DequeueLoopDelay);
                    }
                }
            });

            IPEndPointsUpdateThread.Start();
        }

        public byte[] PackIPEndPoints()
        {
            var data = new byte[IPEndPoints.Count * 6 + 1];
            var index = 0;
            var iPEndPoint = new IPEndPoint(IPAddress.Any, 0);

            for (int i = 0, length = IPEndPoints.Count; i < length; i++)
            {
                iPEndPoint = IPEndPoints[i].iPEndPoint;
                ProByte.Set(ref data, ref index, ProByte.Combine((byte)P2PClientDataType.IPEndPoints, iPEndPoint.Address.GetAddressBytes(), BitConverter.GetBytes((ushort)iPEndPoint.Port)));
            }

            return data;
        }

        #region Data Handlers

        public bool _IPEndPoints(Datagram datagram)
        {
            try
            {
                if (datagram.data.Length != 1)
                    return false;

                var data = datagram.data;
                var iPEndPoint = new IPEndPoint(IPAddress.Any, 0);

                for (int i = 0, length = data.Length; i < length; i += 4)
                {
                    iPEndPoint = new IPEndPoint(IPAddress.Parse($"{data[i++]}.{data[i++]}.{data[i++]}.{data[i++]}"), BitConverter.ToUInt16(data, i));

                    if (IPEndPoints.Contains(new IPEndPointTimePair(iPEndPoint, Time)))
                        continue;

                    Send(iPEndPoint, (byte)P2PClientDataType.AddIPEndPoint);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool AddIPEndPoint(Datagram datagram)
        {
            if (datagram.data.Length != 1)
                return false;

            var iPEndPointTimePair = new IPEndPointTimePair(datagram.iPEndPoint, Time);

            if (!IPEndPoints.Contains(iPEndPointTimePair))
            {
                IPEndPoints.Add(iPEndPointTimePair);
                Log.Show($"Added new client {datagram.iPEndPoint}!", "AddIPEndPoint");
            }

            return true;
        }

        public bool GetIPEndPoints(Datagram datagram)
        {
            if (datagram.data.Length != 1)
                return false;

            Send(datagram.iPEndPoint, PackIPEndPoints());

            return true;
        }

        #endregion
    }
}
