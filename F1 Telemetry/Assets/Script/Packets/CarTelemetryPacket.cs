using UnityEngine;

public class CarTelemetryPacket : Packet
{
    public CarTelemetryPacket(byte[] data) : base(data) { }

    public override void LoadBytes()
    {
        base.LoadBytes();
    }
}
