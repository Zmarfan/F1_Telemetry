using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacketManager : MonoBehaviour
{
    static Queue<Packet> _packets = new Queue<Packet>();

    public static void AddPacket(Packet newPackage)
    {
        _packets.Enqueue(newPackage);
    }

    private void Update()
    {
        while (_packets.Count > 0)
        {
            Packet packet = _packets.Dequeue();
            packet.LoadBytes();
        }
    }
}
