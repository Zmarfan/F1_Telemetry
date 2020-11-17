using UnityEngine;

public class SessionPacket : Packet
{
    public SessionPacket(byte[] data) : base(data) { }

    public override void LoadBytes()
    {
        base.LoadBytes();
        Debug.Log("I AM SESSION_PACKET!");
    }
}
