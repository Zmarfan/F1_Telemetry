using System.Net;
using System.Net.Sockets;
using UnityEngine;
using System.Collections;
using System;

public struct UdpState
{
    public UdpClient client;
    public IPEndPoint endpoint;
}

/// <summary>
/// Receiver of UDP packets sent from F1 2020 by codemasters into port 20777.
/// After receive -> transmit data to PacketManager that handles it further.
/// </summary>
public class UdpReceiver : MonoBehaviour
{
    static readonly int PORT = 20777;     //Entry port for data from the game
    static byte[] _data;                  //the variable where data from the game is temp stored from each packet

    static bool _messageReceived = false; //Makes sure we only read data when new data has come in

    void Awake()
    {
        UdpClient client = new UdpClient(PORT);
        IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, PORT);

        UdpState state = new UdpState();
        state.client = client;
        state.endpoint = endpoint;

        StartCoroutine(Listening(state, client));
    }

    /// <summary>
    /// Loop which listens, receives and transmits further data
    /// </summary>
    IEnumerator Listening(UdpState state, UdpClient client)
    {
        Debug.Log("Start listening for incoming messages!");

        while (true)
        {
            //Start listening for new data
            client.BeginReceive(new AsyncCallback(ReceiveCallback), state);
 
            //Waits until new data has arrived
            while (!_messageReceived)
                yield return new WaitForEndOfFrame();
            //When data is received it's stored in _data variable!

            byte[] copyData = new byte[_data.Length];
            //Copy data as to not mix up references -> keep data secured
            for (int i = 0; i < _data.Length; i++)
                copyData[i] = _data[i];

            PacketManager.AddPacketData(_data);

            //Done with this message, wait for next
            _messageReceived = false;
            yield return new WaitForEndOfFrame();
        }
    }

    public static void ReceiveCallback(IAsyncResult ar)
    {
        UdpClient client = ((UdpState)(ar.AsyncState)).client;
        IPEndPoint endpoint = ((UdpState)(ar.AsyncState)).endpoint;

        _data = client.EndReceive(ar, ref endpoint);
        _messageReceived = true;
    }
}
