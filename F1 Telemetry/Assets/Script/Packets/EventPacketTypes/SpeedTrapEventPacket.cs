using System;

/// <summary>
/// This packet gives details of Speed Trap event.
/// Packet sent when event occurs
/// </summary>
public class SpeedTrapEventPacket : EventPacket
{
    public byte VehicleIndex { get; private set; }
    public float Speed { get; private set; }       //Top speed achived in km/h

    public SpeedTrapEventPacket(byte[] data) : base(data) { }

    public override void LoadBytes()
    {
        base.LoadBytes();

        ByteManager manager = new ByteManager(Data, MOVE_PAST_EVENT_HEADER);

        VehicleIndex = manager.GetByte();
        Speed = BitConverter.ToSingle(manager.GetBytes(sizeof(float)), 0);
    }
}
