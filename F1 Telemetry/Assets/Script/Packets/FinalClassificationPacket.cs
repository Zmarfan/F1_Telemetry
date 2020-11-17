using UnityEngine;

public class FinalClassificationPacket : Packet
{
    public FinalClassificationPacket(byte[] data) : base(data) { }

    public override void LoadBytes()
    {
        base.LoadBytes();
        Debug.Log("I AM FINAL_CLASSIFICATION_PACKET!");
    }
}
