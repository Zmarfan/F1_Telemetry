
/// <summary>
/// This packet gives details of Fastest lap event.
/// Packet sent when event occurs
/// </summary>
public class FastestLapEventPacket : EventPacket
{
    public FastestLapEventPacket(byte[] data) : base(data) { }

    public override void LoadBytes()
    {
        base.LoadBytes();
    }
}
