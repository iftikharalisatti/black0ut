// --------------------------------
// Author - Black0uter (Nick)
//
// Copyright (C) 2018
// GNU GENERAL PUBLIC LICENSE V3
// --------------------------------

using System;
using System.Net;
using System.Threading;
using System.Net.Sockets;
using System.Collections.Generic;

namespace Black0ut.Net
{
    public class QueuedUDPClient : UdpClient
    {
        public int DequeueLoopDelay;

        public List<IPEndPoint> MutedIPEndPoints;

        public Queue<Datagram> DatagramQueue;

        public Thread ReveiveThread;
        public Thread DequeueThread;

        public DLogHandler LogHandler;

        public DataHandler DataHandler;

        public QueuedUDPClient(string hostname, int port, int dequeueLoopDelay = 10) : base(hostname, port)
        {
            DequeueLoopDelay = dequeueLoopDelay;

            DataHandler = new DataHandler();

            DatagramQueue = new Queue<Datagram>();

            ReveiveThread = new Thread(() =>
            {
                LogHandler($"Started thread ReveiveThread.", "");

                var datagram = new Datagram();

                while (true)
                {
                    try
                    {
                        datagram.Data = Receive(ref datagram.iPEndPoint);

                        if(!MutedIPEndPoints.Contains(datagram.iPEndPoint))
                            DatagramQueue.Enqueue(datagram);
                    }
                    catch (Exception e)
                    {
                        LogHandler(e.ToString(), "");
                    }
                }
            });

            DequeueThread = new Thread(() =>
            {
                LogHandler($"Started thread DequeueThread.", "");

                var datagram = new Datagram();

                while (true)
                {
                    try
                    {
                        if (DatagramQueue.Count == 0)
                            return;

                        for (int i = 0, length = DatagramQueue.Count; i < length; i++)
                        {
                            datagram = DatagramQueue.Dequeue();

                            if (!DataHandler.Handle(datagram) && !MutedIPEndPoints.Contains(datagram.iPEndPoint))
                            {
                                MutedIPEndPoints.Add(datagram.iPEndPoint);
                                LogHandler($"IPEndPoint '{datagram.iPEndPoint}' added to MutedIPEndPoints! Failed to handle received data '{BitConverter.ToString(datagram.Data)}'.", "");
                            }
                        }
                        
                        Thread.Sleep(DequeueLoopDelay);
                    }
                    catch (Exception e)
                    {
                        LogHandler(e.ToString(), "");
                    }
                }
            });

            ReveiveThread.Start();
            DequeueThread.Start();
        }

        public void Close()
        {
            ReveiveThread.Interrupt();
            DequeueThread.Interrupt();

            Close();

            LogHandler("Stopped threads and closed UdpClient connection.", "Close");
        }

        public void Send(IPEndPoint iPEndPoint, byte[] data)
        {
            Send(data, data.Length, iPEndPoint);
        }

        public void Send(IPEndPoint iPEndPoint, byte dataType)
        {
            Send(new byte[1] { dataType }, 1, iPEndPoint);
        }
    }
}