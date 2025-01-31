﻿namespace F1_Data_Management
{
    /// <summary>
    /// This packet gives details of Team mate in pits event.
    /// Packet sent when event occurs
    /// </summary>
    public class TeamMateInPitsEventPacket : EventPacket
    {
        public byte VehicleIndex { get; private set; } //Vehicle index of the car that entered the pits

        public TeamMateInPitsEventPacket(byte[] data) : base(data) { }

        protected override void LoadBytes()
        {
            base.LoadBytes();

            ByteManager manager = new ByteManager(Data, MOVE_PAST_EVENT_HEADER, "Team mate in pits event packet");
            VehicleIndex = manager.GetByte();
        }
    }
}