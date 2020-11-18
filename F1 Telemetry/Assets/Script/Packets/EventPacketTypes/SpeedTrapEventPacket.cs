
/// <summary>
/// This packet gives details of Speed Trap event.
/// Packet sent when event occurs
/// </summary>
public class SpeedTrapEventPacket : EventPacket
{
    public SpeedTrapEventPacket(byte[] data) : base(data) { }

    public override void LoadBytes()
    {
        base.LoadBytes();
    }
}
