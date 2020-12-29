namespace F1_Data_Management
{
    /// <summary>
    /// This packet gives details of Race Winner event.
    /// Packet sent when event occurs
    /// </summary>
    public class RaceWinnerEventPacket : EventPacket
    {
        public byte VehicleIndex { get; private set; } //Vehicle Index of the car that won

        public RaceWinnerEventPacket(byte[] data) : base(data) { }

        protected override void LoadBytes()
        {
            base.LoadBytes();

            ByteManager manager = new ByteManager(Data, MOVE_PAST_EVENT_HEADER, "Race winner packet");
            VehicleIndex = manager.GetByte();
        }
    }
}