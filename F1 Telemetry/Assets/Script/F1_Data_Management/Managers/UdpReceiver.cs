using System.Net;
using System.Net.Sockets;
using UnityEngine;
using System.Threading;

namespace F1_Data_Management
{
    /// <summary>
    /// Receiver of UDP packets sent from F1 2020 by codemasters into port 20777.
    /// After receive -> transmit data to PacketManager that handles it further.
    /// </summary>
    public class UdpReceiver
    {
        PacketManager _packetManager;

        readonly int _port = 20777;     //Entry port for data from the game 20777
        UdpClient _udp;
        Thread _thread;

        readonly object _lockObject = new object();
        byte[] _returnData;

        public bool CurrentlyListening { get; private set; } = false;

        public UdpReceiver(PacketManager packetManager, int portNumber = 20777)
        {
            _packetManager = packetManager;
            _port = portNumber;
        }

        ~UdpReceiver()
        {
            StopListening();
        }

        /// <summary>
        /// Starts process of listening for Packets from F1 2020 and then processing and storing the data.
        /// </summary>
        public void StartListening()
        {
            CurrentlyListening = true;
            _thread = new Thread(new ThreadStart(ThreadMethod));
            _thread.Start();
        }

        /// <summary>
        /// Stops process of listening for Packets from F1 2020 and then processing and storing the data.
        /// </summary>
        public void StopListening()
        {
            CurrentlyListening = false;
            _thread.Abort();
        }

        /// <summary>
        /// Thread which listens on port and sends packets to PacketManager
        /// </summary>
        void ThreadMethod()
        {
            _udp = new UdpClient(_port);
            while (true)
            {
                IPEndPoint remoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = _udp.Receive(ref remoteIpEndPoint); //Receive data from port

                lock (_lockObject) //Locking the data does nothing of value right now but is good practice :3
                {
                    _returnData = data;

                    //Copy data as to not mix up references -> keep data secured
                    byte[] copyData = new byte[_returnData.Length];
                    for (int i = 0; i < _returnData.Length; i++)
                        copyData[i] = _returnData[i];

                    //Sends data packet to PacketManager for further processing
                    _packetManager.AddPacketData(_returnData);
                }
            }
        }
    }
}
