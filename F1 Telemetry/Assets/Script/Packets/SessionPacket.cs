using UnityEngine;

public class SessionPacket : Packet
{
    public Weather Weather { get; protected set; }
    public sbyte TrackTemperature { get; protected set; }
    public sbyte AirTemperature { get; protected set; }
    public byte TotalLaps { get; protected set; }
    public SessionType SessionType { get; protected set; } 

    public SessionPacket(byte[] data) : base(data) { }

    public override void LoadBytes()
    {
        base.LoadBytes();
        ByteManager manager = new ByteManager(Data, MOVE_PAST_HEADER_INDEX);
    }
}
