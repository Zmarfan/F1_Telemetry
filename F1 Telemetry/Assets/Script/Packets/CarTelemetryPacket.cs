using UnityEngine;

public class CarTelemetryPacket : Packet
{
    public CarTelemetryPacket(byte[] data) : base(data) { }

    public override void LoadBytes()
    {
        base.LoadBytes();
        Debug.Log("I AM CAR_TELEMETRY_PACKET!");
    }
}
