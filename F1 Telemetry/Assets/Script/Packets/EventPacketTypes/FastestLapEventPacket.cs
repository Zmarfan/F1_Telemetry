using UnityEngine;
using System;

/// <summary>
/// This packet gives details of Fastest lap event.
/// Packet sent when event occurs
/// </summary>
public class FastestLapEventPacket : EventPacket
{
    public byte VehicleIndex { get; private set; } 
    public float LapTime { get; private set; }     //Lap time in seconds

    public FastestLapEventPacket(byte[] data) : base(data) { }

    public override void LoadBytes()
    {
        base.LoadBytes();

        ByteManager manager = new ByteManager(Data, MOVE_PAST_EVENT_HEADER);

        VehicleIndex = manager.GetByte();
        LapTime = BitConverter.ToSingle(manager.GetBytes(sizeof(float)), 0);
    }
}
