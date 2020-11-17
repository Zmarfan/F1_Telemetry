using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacketManager : MonoBehaviour
{
    static Queue<Packet> _packets = new Queue<Packet>(); //Queue of all packets received since last frame

    /// <summary>
    /// Add a packet to the queue that will be processed next frame
    /// </summary>
    public static void AddPacket(Packet newPackage)
    {
        _packets.Enqueue(newPackage);
    }

    private void Update()
    {
        //Handle all the packets that have come in since last frame
        while (_packets.Count > 0)
            HandlePackage(_packets.Dequeue());
    }

    /// <summary>
    /// Identifies packet type and handles it ackoringly -> Read data from packet and transmits it further
    /// </summary>
    void HandlePackage(Packet packet)
    {
        packet = GetPacketType(packet);
        packet.LoadBytes();
    }

    /// <summary>
    /// Id what type of packet it is and convert it to that typ of package
    /// </summary>
    Packet GetPacketType(Packet packet)
    {
        PacketType packetType = (PacketType)packet.GetPacketIndex();

        switch (packetType)
        {
            case (PacketType.MOTION): { return new MotionPacket(packet.Data); }
            default: { Debug.LogWarning("Packet type is not yet supported!"); return packet; }
        }
    }
}
