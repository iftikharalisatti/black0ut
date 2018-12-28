using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Black0ut.GameNet
{
    using Log;
    using Byte;

    public class GameUdpClient : UdpClient
    {
        public string Name;
        public IPEndPoint MainSender;
        public int SendersCount;
        public long RejectTime;
        public List<GameSender> Senders;
        public object ReceiveLocker;
        public long Time;
        public Thread ReceiveThread;
        public Thread TimeThread;
        public Thread RejectThread;
        public Action<GamePacket, GameSender> OnPacketReceive;
        public Action<GameSender> OnMainSenderAdd;
        public Action<GameSender> OnSenderAdd;
        public Action<GameSender> OnMainSenderRemove;
        public Action<GameSender> OnSenderRemove;

        public GameUdpClient(int port, IPEndPoint mainSender, int sendersCount, long rejectTime) : base(port)
        {
            MainSender = mainSender;
            SendersCount = sendersCount;
            RejectTime = rejectTime;
            Senders = new List<GameSender>();
            ReceiveLocker = new object();
            Time = 0;
            ReceiveThread = new Thread(() =>
            {
                Log.Show($"Thread started.", "ReceiveThread");
                var packet = new GamePacket();
                while (true)
                {
                    try
                    {
                        packet = new GamePacket();
                        packet.Data = Receive(ref packet.Sender);
                        packet.ReceiveTime = Time;
                        lock (ReceiveLocker)
                        {
                            if (mainSender != null && !_MainSenderEquals(packet.Sender))
                            {
                                Log.Show($"Received packet from not main sender [{MainSender}], sender [{packet.Sender}].", "ReceiveThread");
                                continue;
                            }
                            if (packet.Data.Length == 1 && packet.Data[0] == 0)
                            {
                                if (!Senders.Contains(new GameSender(packet.Sender, Time)))
                                {
                                    if (Senders.Count < SendersCount)
                                    {
                                        var sender = new GameSender(packet.Sender, Time);
                                        if (MainSender != null && _MainSenderEquals(sender))
                                        {
                                            OnMainSenderAdd?.Invoke(sender);
                                            Log.Show($"Connected to main sender [{sender}].", "ReceiveThread");
                                        }
                                        else
                                        {
                                            OnSenderAdd?.Invoke(sender);
                                            Send(sender, 0);
                                            Log.Show($"Connected new sender [{sender}].", "ReceiveThread");
                                        }
                                        Senders.Add(sender);
                                    }
                                    else
                                        Log.Show($"Senders list is full, sender [{packet.Sender}].", "ReceiveThread");
                                }
                                else
                                    Log.Show($"Sender already connected, sender [{packet.Sender}].", "ReceiveThread");
                                continue;
                            }
                            if (Senders.Contains(new GameSender(packet.Sender, Time)))
                                OnPacketReceive?.Invoke(packet, Senders[Senders.IndexOf(new GameSender(packet.Sender, Time))]);
                            else
                                Log.Show($"Received packet from not connected sender [{packet.Sender}].", "ReceiveThread");
                        }
                    }
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
            RejectThread = new Thread(() =>
            {
                Log.Show($"Thread started.", "RejectThread");
                while (true)
                {
                    lock (ReceiveThread)
                    {
                        for (int i = 0; i < Senders.Count; i++)
                        {
                            if (Time - Senders[i].LastReceiveTime > RejectTime)
                            {
                                if (MainSender != null && _MainSenderEquals(Senders[i]))
                                {
                                    OnMainSenderRemove?.Invoke(Senders[i]);
                                    Log.Show($"Rejected main sender [{Senders[i]}].", "RejectThread");
                                }
                                else
                                {
                                    OnSenderRemove?.Invoke(Senders[i]);
                                    Log.Show($"Rejected sender [{Senders[i]}].", "RejectThread");
                                }
                                Senders.Remove(Senders[i]);
                            }
                        }
                    }
                    Thread.Sleep(1);
                }
            });
            RejectThread.Start();
            Log.Show($"Created GameUdpClient instance. LocalEndPoint [{Client.LocalEndPoint}].", "GameUdpClient");
        }

        /// <summary>
        /// Client
        /// </summary>
        public GameUdpClient(int port, IPEndPoint mainSender, long rejectTime) : this(port, mainSender, 1, rejectTime) { }

        /// <summary>
        /// Server
        /// </summary>
        public GameUdpClient(int port, int sendersCount, long rejectTime) : this(port, null, sendersCount, rejectTime) { }

        public bool _MainSenderEquals(IPEndPoint sender)
        {
            return MainSender.Port == sender.Port && BytePro.Equals(MainSender.Address.GetAddressBytes(), sender.Address.GetAddressBytes());
        }

        public void Stop()
        {
            ReceiveThread.Abort();
            TimeThread.Abort();
            RejectThread.Abort();
            Close();
            Log.Show($"Stopped GameUdpClient.", "Stop");
        }

        public void Connect()
        {
            if (MainSender != null)
                Send(0);
            else
                Log.Show("Can not connect to main sender, main sender is null.", "Connect");
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
            Send(new byte[] { packetType }, 1, endPoint);
        }

        public void Send(params byte[][] datas)
        {
            Send(MainSender, datas);
        }
        public void Send(byte packetType, params byte[][] datas)
        {
            Send(MainSender, packetType, datas);
        }
        public void Send(byte packetType)
        {
            Send(MainSender, packetType);
        }

        #endregion
    }
}
