
/// <summary>
/// This packet gives details of Race Winner event.
/// Packet sent when event occurs
/// </summary>
public class RaceWinnerEventPacket : EventPacket
{
    public RaceWinnerEventPacket(byte[] data) : base(data) { }

    public override void LoadBytes()
    {
        base.LoadBytes();
    }
}
