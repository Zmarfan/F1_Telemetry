using UnityEngine;

public class FinalClassificationPacket : Packet
{
    public FinalClassificationPacket(byte[] data) : base(data) { }

    public override void LoadBytes()
    {
        base.LoadBytes();
    }
}
