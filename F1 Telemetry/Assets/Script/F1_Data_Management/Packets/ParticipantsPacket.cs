namespace F1_Data_Management
{
    /// <summary>
    /// This is a list of participants in the race. If controlled by AI -> name is driver name, online, name -> steam 
    /// 1 packet every 5 seconds
    /// </summary>
    public class ParticipantsPacket : Packet
    {
        public byte NumberOfActiveCars { get; private set; }
        public ParticipantData[] AllParticipantData { get; private set; }

        public ParticipantsPacket(byte[] data) : base(data) { }

        /// <summary>
        /// Loads in Header data and Participant data, must run before working on data
        /// </summary>
        protected override void LoadBytes()
        {
            base.LoadBytes();
            ByteManager manager = new ByteManager(Data, MOVE_PAST_HEADER_INDEX, "Participation packet");

            NumberOfActiveCars = manager.GetByte();
            AllParticipantData = new ParticipantData[F1Info.MAX_AMOUNT_OF_CARS];

            //Read all instances of ParticipantData[] in the data -> It's all linear
            for (int i = 0; i < AllParticipantData.Length; i++)
            {
                AllParticipantData[i].AIControlled = manager.GetEnumFromByte<ControlledStatus>();
                AllParticipantData[i].driverID = manager.GetByte();
                AllParticipantData[i].team = manager.GetEnumFromByte<Team>();
                AllParticipantData[i].raceNumber = manager.GetByte();
                AllParticipantData[i].nationality = manager.GetEnumFromByte<Nationality>();
                AllParticipantData[i].name = manager.GetString(ParticipantData.AMOUNT_OF_CHARS_IN_NAME);
                AllParticipantData[i].publicTelemetry = manager.GetBool();

                //Added calculated data not present in packet from f1 2020

                AllParticipantData[i].vehicleIndex = i;
            }
        }
    }

    /// <summary>
    /// Holds ParticipantData for one driver in the race
    /// </summary>
    [System.Serializable]
    public struct ParticipantData
    {
        public ControlledStatus AIControlled;
        public byte driverID;                  //100 if player
        public Team team;
        public byte raceNumber;
        public Nationality nationality;
        public string name;                    //last name for AI, Player for players, (not to be used, good for debugging though)
        public bool publicTelemetry;           //true if it is public
        public int vehicleIndex;

        public static readonly int AMOUNT_OF_CHARS_IN_NAME = 48; //Amount of bytes to make up name in ParticipantData / Packet
    }
}