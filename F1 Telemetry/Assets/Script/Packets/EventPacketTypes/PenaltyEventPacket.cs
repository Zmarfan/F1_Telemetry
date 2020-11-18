
/// <summary>
/// This packet gives details of Penalty event.
/// Packet sent when event occurs
/// </summary>
public class PenaltyEventPacket : EventPacket
{
    public PenaltyEventPacket(byte[] data) : base(data) { }

    public override void LoadBytes()
    {
        base.LoadBytes();
    }
}
