namespace F1_Data_Management
{
    /// <summary>
    /// This packet gives details of Fastest lap event.
    /// Packet sent when event occurs
    /// </summary>
    public class FastestLapEventPacket : EventPacket
    {
        public byte VehicleIndex { get; private set; }
        public float LapTime { get; private set; }     //Lap time in seconds

        public FastestLapEventPacket(byte[] data) : base(data) { }

        protected override void LoadBytes()
        {
            base.LoadBytes();

            ByteManager manager = new ByteManager(Data, MOVE_PAST_EVENT_HEADER, "Fastest lap packet");

            VehicleIndex = manager.GetByte();
            LapTime = manager.GetFloat();
        }
    }
}