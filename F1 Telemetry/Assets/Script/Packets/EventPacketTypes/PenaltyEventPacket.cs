using UnityEngine;

/// <summary>
/// This packet gives details of Penalty event.
/// Packet sent when event occurs
/// </summary>
public class PenaltyEventPacket : EventPacket
{
    public PenaltyType PenaltyType { get; private set; }
    public InfringementType InfringementType { get; private set; }
    public byte VehicleIndex { get; private set; }
    public byte OtherVehicleIndex { get; private set; }             //Vehicle Index of the other car involved in incident
    public byte Time { get; private set; }                          //Either time gained, or time spent doing action. In seconds! (255 if not relevant)
    public byte LapNumber { get; private set; }                     //Lap the penalty occurred on
    public byte PlacesGained { get; private set; }      

    public PenaltyEventPacket(byte[] data) : base(data) { }

    public override void LoadBytes()
    {
        base.LoadBytes();

        ByteManager manager = new ByteManager(Data, MOVE_PAST_EVENT_HEADER, "Penalty Event Packet");

        PenaltyType = (PenaltyType)manager.GetByte();
        InfringementType = (InfringementType)manager.GetByte();
        VehicleIndex = manager.GetByte();
        OtherVehicleIndex = manager.GetByte();
        Time = manager.GetByte();
        LapNumber = manager.GetByte();
        PlacesGained = manager.GetByte();
    }
}
