using System.Net;
using System.Net.Sockets;
using UnityEngine;
using System.Collections;
using System;
using System.Text;
using System.Threading;

public struct UdpState
{
    public UdpClient client;
    public IPEndPoint endpoint;
}

public class UdpReceiver : MonoBehaviour
{
    static readonly int PORT = 20777;

    static bool messageReceived = false;

    void Awake()
    {
        UdpClient client = new UdpClient(PORT);
        IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, PORT);

        UdpState state = new UdpState();
        state.client = client;
        state.endpoint = endpoint;

        Debug.Log("Trying to connect!");

        client.BeginReceive(new AsyncCallback(ReceiveCallback), state);

        StartCoroutine(Waiting());
    }

    IEnumerator Waiting()
    {
        while (!messageReceived)
        {
            Debug.Log("Waiting for message");
            yield return new WaitForEndOfFrame();
        }
    }

    public static void ReceiveCallback(IAsyncResult ar)
    {
        UdpClient client = ((UdpState)(ar.AsyncState)).client;
        IPEndPoint endpoint = ((UdpState)(ar.AsyncState)).endpoint;

        byte[] receiveBytes = client.EndReceive(ar, ref endpoint);
        //string receiveString = Encoding.BigEndianUnicode.GetString(receiveBytes);

        byte[] packetFormat = new byte[2];
        packetFormat[0] = receiveBytes[0];
        packetFormat[1] = receiveBytes[1];

        ushort message = BitConverter.ToUInt16(packetFormat, 0);

        Debug.Log(message);

        messageReceived = true;
    }
}
