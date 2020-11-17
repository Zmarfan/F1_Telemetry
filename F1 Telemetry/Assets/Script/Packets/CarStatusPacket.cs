using UnityEngine;

public class CarStatusPacket : Packet
{
    public CarStatusPacket(byte[] data) : base(data) { }

    public override void LoadBytes()
    {
        base.LoadBytes();
        Debug.Log("I AM CAR_STATUS_PACKET!");
    }
}
