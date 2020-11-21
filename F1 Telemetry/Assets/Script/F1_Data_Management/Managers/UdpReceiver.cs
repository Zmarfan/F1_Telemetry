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
        static readonly int PORT = 20777;     //Entry port for data from the game
        static UdpClient udp;
        static Thread thread;

        static readonly object _lockObject = new object();
        static byte[] _returnData;

        public static bool CurrentlyListening { get; private set; } = false;

        /// <summary>
        /// Starts process of listening for Packets from F1 2020 and then processing and storing the data.
        /// </summary>
        public static void StartListening()
        {
            CurrentlyListening = true;
            thread = new Thread(new ThreadStart(ThreadMethod));
            thread.Start();
        }

        /// <summary>
        /// Stops process of listening for Packets from F1 2020 and then processing and storing the data.
        /// </summary>
        public static void StopListening()
        {
            CurrentlyListening = false;
            thread.Abort();
        }

        /// <summary>
        /// Thread which listens on port and sends packets to PacketManager
        /// </summary>
        static void ThreadMethod()
        {
            udp = new UdpClient(PORT);
            while (true)
            {
                IPEndPoint remoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = udp.Receive(ref remoteIpEndPoint); //Receive data from port

                lock (_lockObject) //Locking the data does nothing of value right now but is good practice :3
                {
                    _returnData = data;

                    //Copy data as to not mix up references -> keep data secured
                    byte[] copyData = new byte[_returnData.Length];
                    for (int i = 0; i < _returnData.Length; i++)
                        copyData[i] = _returnData[i];

                    //Sends data packet to PacketManager for further processing
                    PacketManager.AddPacketData(_returnData);
                }
            }
        }
    }
}
