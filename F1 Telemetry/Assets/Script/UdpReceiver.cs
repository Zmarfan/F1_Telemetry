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

public class UdpReceiver : MonoBehaviour
{
    static readonly int PORT = 20777;
    static byte[] _data;

    static bool _messageReceived = false;

    void Awake()
    {
        UdpClient client = new UdpClient(PORT);
        IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, PORT);

        UdpState state = new UdpState();
        state.client = client;
        state.endpoint = endpoint;

        StartCoroutine(Listening(state, client));
    }

    IEnumerator Listening(UdpState state, UdpClient client)
    {
        Debug.Log("Start listening for incoming messages!");

        while (true)
        {
            client.BeginReceive(new AsyncCallback(ReceiveCallback), state);
 
            while (!_messageReceived)
            {
                Debug.Log("Waiting for incoming message!");
                yield return new WaitForEndOfFrame();
            }
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
