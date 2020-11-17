using UnityEngine;

public class CarSetupPacket : Packet
{
    public CarSetupPacket(byte[] data) : base(data) { }

    public override void LoadBytes()
    {
        base.LoadBytes();
        Debug.Log("I AM CAR_SETUP_PACKET!");
    }
}