using UnityEngine;

public class MotionPacket : Packet
{
    public MotionPacket(byte[] data) : base(data) { }

    public override void LoadBytes()
    {
        base.LoadBytes();
    }
}
