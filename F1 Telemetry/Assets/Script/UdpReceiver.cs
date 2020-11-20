using System.Net;
using System.Net.Sockets;
using UnityEngine;
using System.Threading;

/// <summary>
/// Receiver of UDP packets sent from F1 2020 by codemasters into port 20777.
/// After receive -> transmit data to PacketManager that handles it further.
/// </summary>
public class UdpReceiver : MonoBehaviour
{
    static readonly int PORT = 20777;     //Entry port for data from the game
    static UdpClient udp;
    Thread thread;

    static readonly object _lockObject = new object();
    byte[] _returnData;

    private void Start()
    {
        thread = new Thread(new ThreadStart(ThreadMethod));
        thread.Start();
    }

    private void ThreadMethod()
    {
        udp = new UdpClient(PORT);
        while (true)
        {
            IPEndPoint remoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] data = udp.Receive(ref remoteIpEndPoint);

            lock (_lockObject)
            {
                _returnData = data;

                byte[] copyData = new byte[_returnData.Length];
                //Copy data as to not mix up references -> keep data secured
                for (int i = 0; i < _returnData.Length; i++)
                    copyData[i] = _returnData[i];

                PacketManager.AddPacketData(_returnData);
            }
        }
    }
}
