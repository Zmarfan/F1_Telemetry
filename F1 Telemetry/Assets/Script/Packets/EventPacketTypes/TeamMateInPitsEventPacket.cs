
/// <summary>
/// This packet gives details of Team mate in pits event.
/// Packet sent when event occurs
/// </summary>
public class TeamMateInPitsEventPacket : EventPacket
{
    public TeamMateInPitsEventPacket(byte[] data) : base(data) { }

    public override void LoadBytes()
    {
        base.LoadBytes();
    }
}

