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
    using Byte;

    public class NetUdpClient<TClientCustomData> : UdpClient
    {
        public IPEndPoint ServerEndPoint;
        public int MaxClientsCount;
        public long ClientRejectTime;

        public long Time;
        public object ReceiveLocker;

        public List<NetClient<TClientCustomData>> Clients;

        public Thread ReceiveThread;
        public Thread TimeThread;

        public Thread RejectThread;

        public Action<NetPacket, NetClient<TClientCustomData>> OnClientAdd;
        public Action<NetPacket, NetClient<TClientCustomData>> OnPacketReceive;

        public Action<NetClient<TClientCustomData>> OnClientRemove;

        #region .ctor

        public NetUdpClient(int port, IPEndPoint serverEndPoint, int maxClientsCount, long clientRejectTime = 10000) : base(port)
        {
            ServerEndPoint = serverEndPoint;
            MaxClientsCount = maxClientsCount;
            ClientRejectTime = clientRejectTime;

            Time = 0;
            ReceiveLocker = new object();

            Clients = new List<NetClient<TClientCustomData>>();

            ReceiveThread = new Thread(() =>
            {
                Log.Show($"Thread started.", "ReceiveThread");

                var packet = new NetPacket();

                while (true)
                {
                    try
                    {
                        packet = new NetPacket();
                        packet.Data = Receive(ref packet.EndPoint);
                        packet.ReceiveTime = Time;

                        lock (ReceiveLocker)
                        {
                            if(packet.Data.Length == 0)
                            {
                                Log.Show($"receiveed empty data array from client [{packet.EndPoint}].", "ReceiveThread");
                                continue;
                            }

                            if (ServerEndPoint == null)
                            {
                                var client = new NetClient<TClientCustomData>(packet.EndPoint, Time);

                                // CONNECT REQUEST
                                if (packet.Data[0] == 0)
                                {
                                    if (Clients.Contains(client))
                                    {
                                        Log.Show($"Received packet from already connected client [{packet.EndPoint}].", "ReceiveThread");
                                        continue;
                                    }

                                    if (Clients.Count >= MaxClientsCount)
                                    {
                                        Log.Show($"Clients list is full, client [{packet.EndPoint}].", "ReceiveThread");
                                        continue;
                                    }

                                    OnClientAdd?.Invoke(packet, client);

                                    Log.Show($"Connected new client [{client}].", "ReceiveThread");
                                    continue;
                                }

                                if (!Clients.Contains(client))
                                {
                                    Log.Show($"Received packet from not connected client [{packet.EndPoint}]. Data [{BitConverter.ToString(packet.Data)}]", "ReceiveThread");
                                    continue;
                                }

                                // SERVER
                                var _client = Clients[Clients.IndexOf(client)];

                                _client.LastReceiveTime = Time;
                                OnPacketReceive?.Invoke(packet, _client);
                            }
                            else
                            {
                                if (!EndPointPro.Equals(serverEndPoint, packet.EndPoint))
                                {
                                    Log.Show($"Received packet from not server EndPoint [{packet.EndPoint}].", "ReceiveThread");
                                    continue;
                                }

                                // CLIENT
                                OnPacketReceive?.Invoke(packet, null);
                            }
                        }
                    }
                    catch (SocketException) { }
                    catch (Exception e)
                    {
                        Log.Show($"Exception [{e}]", "ReceiveThread");
                    }
                }
            });

            ReceiveThread.Start();

            TimeThread = new Thread(() =>
            {
                Log.Show($"Thread started.", "TimeThread");

                while (true)
                {
                    lock (ReceiveLocker)
                    {
                        Time++;
                    }

                    Thread.Sleep(1);
                }
            });

            TimeThread.Start();

            // EXTENDED THREADS
            RejectThread = new Thread(() =>
            {
                Log.Show($"Thread started.", "RejectThread");

                NetClient<TClientCustomData> client;

                while (true)
                {
                    lock (ReceiveThread)
                    {
                        for (int i = 0; i < Clients.Count; i++)
                        {
                            client = Clients[i];

                            if (Time - client.LastReceiveTime < ClientRejectTime)
                                continue;

                            OnClientRemove?.Invoke(client);
                            Log.Show($"Rejected client [{client}].", "RejectThread");
                        }
                    }

                    Thread.Sleep(1);
                }
            });

            Log.Show($"Instance created. LocalEndPoint [{Client.LocalEndPoint}].", "GameUdpClient");
        }

        /// <summary>
        /// Client
        /// </summary>
        public NetUdpClient(int port, IPEndPoint mainSender, long rejectTime = 10000) : this(port, mainSender, 0, rejectTime) { }

        /// <summary>
        /// Server
        /// </summary>
        public NetUdpClient(int port, int sendersCount, long rejectTime = 10000) : this(port, null, sendersCount, rejectTime) { }

        #endregion

        public void Stop()
        {
            Log.Show($"Stopping net...", "Stop");
            
            ReceiveThread.Abort();
            TimeThread.Abort();
            RejectThread.Abort();

            Close();
        }

        #region Send

        public void Send(IPEndPoint endPoint, params byte[][] datas)
        {
            var data = BytePro.Combine(datas);

            Send(data, data.Length, endPoint);
        }
        public void Send(IPEndPoint endPoint, byte packetType, params byte[][] datas)
        {
            var data = BytePro.Combine(packetType, datas);

            Send(data, data.Length, endPoint);
        }
        public void Send(IPEndPoint endPoint, byte packetType)
        {
            Send(new byte[1] { packetType }, 1, endPoint);
        }

        public void Send(params byte[][] datas)
        {
            Send(ServerEndPoint, datas);
        }
        public void Send(byte packetType, params byte[][] datas)
        {
            Send(ServerEndPoint, packetType, datas);
        }
        public void Send(byte packetType)
        {
            Send(ServerEndPoint, packetType);
        }

        public void SendToAll(byte[][] datas)
        {
            if (Clients.Count < 1)
                return;

            lock (ReceiveLocker)
            {
                for (int i = 0, count = Clients.Count; i < count; i++)
                {
                    Send(Clients[i], datas);
                }
            }
        }
        public void SendToAll(byte packetType, byte[][] datas)
        {
            if (Clients.Count < 1)
                return;

            lock (ReceiveLocker)
            {
                for (int i = 0, count = Clients.Count; i < count; i++)
                {
                    Send(Clients[i], packetType, datas);
                }
            }
        }

        #endregion
    }
}
