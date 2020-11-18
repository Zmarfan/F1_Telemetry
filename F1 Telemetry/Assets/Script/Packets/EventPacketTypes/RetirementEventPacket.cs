
/// <summary>
/// This packet gives details of Retirement event.
/// Packet sent when event occurs
/// </summary>
public class RetirementEventPacket : EventPacket
{
    public byte VehicleIndex { get; private set; } //Vehicle index of the retired car

    public RetirementEventPacket(byte[] data) : base(data) { }

    public override void LoadBytes()
    {
        base.LoadBytes();

        ByteManager manager = new ByteManager(Data, MOVE_PAST_EVENT_HEADER, "Retirement Event Packet");
        VehicleIndex = manager.GetByte();
    }
}
