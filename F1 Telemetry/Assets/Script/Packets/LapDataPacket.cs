using UnityEngine;

public class LapDataPacket : Packet
{
    public LapDataPacket(byte[] data) : base(data) { }

    public override void LoadBytes()
    {
        base.LoadBytes();
        Debug.Log("I AM LAP_DATA!");
    }
}
