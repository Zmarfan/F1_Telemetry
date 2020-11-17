using UnityEngine;

public class EventPacket : Packet
{
    public EventPacket(byte[] data) : base(data) { }

    public override void LoadBytes()
    {
        base.LoadBytes();
        Debug.Log("I AM EVENT_PACKET!");
    }
}