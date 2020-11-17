using UnityEngine;

public class MotionPacket : Packet
{
    public MotionPacket(byte[] data) : base(data) { }

    public override void LoadBytes()
    {
        base.LoadBytes();
        Debug.Log("I AM MOTION_PACKET!");
    }
}
