namespace F1_Data_Management
{
    /// <summary>
    /// This packet gives details of Speed Trap event.
    /// Packet sent when event occurs
    /// </summary>
    public class SpeedTrapEventPacket : EventPacket
    {
        public byte VehicleIndex { get; private set; }
        public float Speed { get; private set; }       //Top speed achived in km/h

        public SpeedTrapEventPacket(byte[] data) : base(data) { }

        public override void LoadBytes()
        {
            base.LoadBytes();

            ByteManager manager = new ByteManager(Data, MOVE_PAST_EVENT_HEADER, "Speed trap event packet");

            VehicleIndex = manager.GetByte();
            Speed = manager.GetFloat();
        }
    }
}