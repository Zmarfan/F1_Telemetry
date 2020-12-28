namespace F1_Data_Management
{
    /// <summary>
    /// This packet details the players currently in a multiplayer lobby. 
    /// 2 packets sent every second when in lobby
    /// </summary>
    public class LobbyInfoPacket : Packet
    {
        public byte NumberOfPlayers { get; private set; }
        public LobbyInfoData[] AllLobbyInfoData { get; private set; }

        public LobbyInfoPacket(byte[] data) : base(data) { }

        public override void LoadBytes()
        {
            base.LoadBytes();

            ByteManager manager = new ByteManager(Data, MOVE_PAST_HEADER_INDEX, "Lobby Info Packet");

            NumberOfPlayers = manager.GetByte();
            AllLobbyInfoData = new LobbyInfoData[F1Info.MAX_AMOUNT_OF_CARS];

            for (int i = 0; i < AllLobbyInfoData.Length; i++)
            {
                AllLobbyInfoData[i].AIControlled = manager.GetBool();
                AllLobbyInfoData[i].team = manager.GetEnumFromByte<Team>();
                AllLobbyInfoData[i].nationality = manager.GetEnumFromByte<Nationality>();
                AllLobbyInfoData[i].name = manager.GetString(LobbyInfoData.AMOUNT_OF_CHARS_IN_NAME);
                AllLobbyInfoData[i].readyStatus = manager.GetEnumFromByte<ReadyStatus>();
            }
        }
    }

    /// <summary>
    /// Holds LobbyInfoData for one driver in the race
    /// </summary>
    public struct LobbyInfoData
    {
        /// <summary>
        /// True if car is controlled by AI
        /// </summary>
        public bool AIControlled;
        public Team team;
        public Nationality nationality;
        public string name;
        public ReadyStatus readyStatus;

        public static readonly int AMOUNT_OF_CHARS_IN_NAME = 48; //Amount of bytes to make up name in LobbyInfoData / Packet
    }
}
