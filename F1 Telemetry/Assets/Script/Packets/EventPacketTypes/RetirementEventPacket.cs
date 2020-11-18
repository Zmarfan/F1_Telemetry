
/// <summary>
/// This packet gives details of Retirement event.
/// Packet sent when event occurs
/// </summary>
public class RetirementEventPacket : EventPacket
{
    public RetirementEventPacket(byte[] data) : base(data) { }

    public override void LoadBytes()
    {
        base.LoadBytes();
    }
}
