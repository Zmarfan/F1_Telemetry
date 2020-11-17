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

            do
            {
                yield return new WaitForEndOfFrame();
                Debug.Log("Waiting for incoming message!");
            } while (!_messageReceived);

            PacketManager.AddPacket(new Packet(_data));

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
