namespace F1_Data_Management
{
    /// <summary>
    /// Base Class for packets, holds header variables shared by all packets and virtual functions for reading data in packet
    /// </summary>
    public class Packet
    {
        protected static readonly int PACKET_ID_INDEX = 5;
        protected static readonly int MOVE_PAST_HEADER_INDEX = 24;

        /// <summary>
        /// The byte array which translates into the accessible variables. No user need.
        /// </summary>
        public byte[] Data { get; protected set; }

        /// <summary>
        /// Game release year
        /// </summary>
        public PacketFormat PacketFormat { get; private set; }
        /// <summary>
        /// X.00
        /// </summary>
        public byte GameMajorVersion { get; private set; }
        /// <summary>
        /// 1.XX
        /// </summary>
        public byte GameMinorVersion { get; private set; }
        public byte PacketVersion { get; private set; }
        /// <summary>
        /// What type of package is this? Identifier. No user need.
        /// </summary>
        public byte PacketID { get; private set; }
        public ulong SessionUniqueID { get; private set; }
        /// <summary>
        /// Total uptime of session
        /// </summary>
        public float SessionTime { get; private set; }
        /// <summary>
        /// Identifier for the frame the data was retrieved on
        /// </summary>
        public uint FrameID { get; private set; }
        /// <summary>
        /// Index of player's car in the array
        /// </summary>
        public byte PlayerCarIndex { get; private set; }
        /// <summary>
        /// Index of secondary player's car in the array (splitscreen) -> 255 if no secondary player
        /// </summary>
        public byte SecondaryPlayerCarIndex { get; private set; }

        public Packet(byte[] data)
        {
            this.Data = data;
        }

        /// <summary>
        /// Returns packet-type of current packet
        /// </summary>
        public static PacketType GetPacketType(byte[] data)
        {
            ByteManager manager = new ByteManager(data, PACKET_ID_INDEX, "Get packet type");
            return manager.GetEnumFromByte<PacketType>();
        }

        /// <summary>
        /// Reads data and set variables, can be overriden by inherited classes (other packet types).
        /// Base packet class read Packet Header only
        /// </summary>
        public virtual void LoadBytes()
        {
            ByteManager manager = new ByteManager(Data, 0, "Header packet");

            PacketFormat = (PacketFormat)manager.GetUnsignedShort(); //Will return 2020 which will be turned to F1_2020 enum
            GameMajorVersion = manager.GetByte();
            GameMinorVersion = manager.GetByte();
            PacketVersion = manager.GetByte();
            PacketID = manager.GetByte();
            SessionUniqueID = manager.GetUnsignedLong();
            SessionTime = manager.GetFloat();
            FrameID = manager.GetUnsignedInt();
            PlayerCarIndex = manager.GetByte();
            SecondaryPlayerCarIndex = manager.GetByte();
        }
    }
}