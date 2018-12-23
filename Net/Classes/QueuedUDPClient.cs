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
    using Log;

    public class QueuedUDPClient : UdpClient
    {
        public Log Log;

        public int DequeueLoopDelay;

        public List<IPEndPoint> MutedIPEndPoints;

        public Queue<Datagram> DatagramQueue;

        public Thread ReveiveThread;
        public Thread DequeueThread;

        public DataHandler DataHandler;

        public QueuedUDPClient(string hostname, int port, Log log, int dequeueLoopDelay = 10) : base(hostname, port)
        {
            Log = log;

            DequeueLoopDelay = dequeueLoopDelay;

            DataHandler = new DataHandler();

            DatagramQueue = new Queue<Datagram>();

            ReveiveThread = new Thread(() =>
            {
                Log.Show($"Started thread ReveiveThread.", "ReveiveThread");

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
                        Log.Show(e.ToString(), "ReveiveThread");
                    }
                }
            });

            DequeueThread = new Thread(() =>
            {
                Log.Show($"Started thread DequeueThread.", "DequeueThread");

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
                                Log.Show($"IPEndPoint '{datagram.iPEndPoint}' added to MutedIPEndPoints! Failed to handle received data '{BitConverter.ToString(datagram.Data)}'.", "DequeueThread");
                            }
                        }
                        
                        Thread.Sleep(DequeueLoopDelay);
                    }
                    catch (Exception e)
                    {
                        Log.Show(e.ToString(), "DequeueThread");
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

            Log.Show("Stopped threads and closed UdpClient connection.", "Close");
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