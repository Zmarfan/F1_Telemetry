namespace F1_Data_Management
{
    /// <summary>
    /// This packet details the final classification at the end of the race
    /// Sent once at end of a race
    /// </summary>
    public class FinalClassificationPacket : Packet
    {
        public byte NumberOfCars { get; private set; }
        public FinalClassificationData[] AllFinalClassificationData { get; private set; }

        public FinalClassificationPacket(byte[] data) : base(data) { }

        public override void LoadBytes()
        {
            base.LoadBytes();

            ByteManager manager = new ByteManager(Data, MOVE_PAST_HEADER_INDEX, "Final Classification Packet");

            NumberOfCars = manager.GetByte();
            AllFinalClassificationData = new FinalClassificationData[F1Info.MAX_AMOUNT_OF_CARS];

            //Read all instances of ParticipantData[] in the data -> It's all linear
            for (int i = 0; i < AllFinalClassificationData.Length; i++)
            {
                AllFinalClassificationData[i].position = manager.GetByte();
                AllFinalClassificationData[i].numberOfLaps = manager.GetByte();
                AllFinalClassificationData[i].gridPosition = manager.GetByte();
                AllFinalClassificationData[i].points = manager.GetByte();
                AllFinalClassificationData[i].numberOfPitStops = manager.GetByte();
                AllFinalClassificationData[i].resultStatus = manager.GetEnumFromByte<ResultStatus>();
                AllFinalClassificationData[i].bestLapTime = manager.GetFloat();
                AllFinalClassificationData[i].totalRaceTime = manager.GetDouble();
                AllFinalClassificationData[i].penaltiesTime = manager.GetByte();
                AllFinalClassificationData[i].numberOfPenalties = manager.GetByte();
                AllFinalClassificationData[i].numberOfTyreStints = manager.GetByte();
                AllFinalClassificationData[i].tyreStintsActual = manager.GetEnumArrayFromBytes<ActualTyreCompound>(Wheel.WHEEL_COUNT);
                AllFinalClassificationData[i].tyreStintsVisual = manager.GetEnumArrayFromBytes<VisualTyreCompound>(Wheel.WHEEL_COUNT);
            }
        }
    }

    /// <summary>
    /// Holds FinalClassificationData for one driver in the race
    /// </summary>
    public struct FinalClassificationData
    {
        /// <summary>
        /// Finishing position
        /// </summary>
        public byte position;
        /// <summary>
        /// Number of completed laps
        /// </summary>
        public byte numberOfLaps;
        /// <summary>
        /// Starting position
        /// </summary>
        public byte gridPosition;
        /// <summary>
        /// points scored based on finishing position and fastest lap
        /// </summary>
        public byte points;
        public byte numberOfPitStops;
        /// <summary>
        /// State the driver finished the race in
        /// </summary>
        public ResultStatus resultStatus;
        /// <summary>
        /// Best lap time in seconds
        /// </summary>
        public float bestLapTime;
        /// <summary>
        /// Total race time in seconds WITHOUT penalties
        /// </summary>
        public double totalRaceTime;
        /// <summary>
        /// All penalties added together in seconds
        /// </summary>
        public byte penaltiesTime;
        /// <summary>
        /// Amount of penalties
        /// </summary>
        public byte numberOfPenalties;
        /// <summary>
        /// Number of tyres stints up to maximum
        /// </summary>
        public byte numberOfTyreStints;
        /// <summary>
        /// Maximum of 8 -> shows C1, C2, C3, C4, C5
        /// </summary>
        public ActualTyreCompound[] tyreStintsActual;
        /// <summary>
        /// Maximum of 8 -> shows soft, medium, hard
        /// </summary>
        public VisualTyreCompound[] tyreStintsVisual;
    }
}