namespace F1_Data_Management
{
    /// <summary>
    /// The Lap data packet gives details of all the cars in the session.
    /// Rate specified in menus (high frequency)
    /// </summary>
    public class LapDataPacket : Packet
    {
        /// <summary>
        /// An array of 22 instances -> If there are less than 22 cars there will be junk values in some instances
        ///Make sure to access via index known to be valid!
        /// </summary>
        public LapData[] LapData { get; private set; }

        public LapDataPacket(byte[] data) : base(data) { }

        protected override void LoadBytes()
        {
            base.LoadBytes();

            ByteManager manager = new ByteManager(Data, MOVE_PAST_HEADER_INDEX, "Lap Data Packet");

            LapData = new LapData[F1Info.MAX_AMOUNT_OF_CARS];

            //This will loop and assign as if there was 22 cars in the race!
            //If there are less cars than 22, these instances will be filled with junk values!
            for (int i = 0; i < F1Info.MAX_AMOUNT_OF_CARS; i++)
            {
                LapData[i].lastLapTime = manager.GetFloat();
                LapData[i].currentLapTime = manager.GetFloat();

                LapData[i].sector1Time = manager.GetUnsignedShort();
                LapData[i].sector2Time = manager.GetUnsignedShort();
                LapData[i].bestLapTime = manager.GetFloat();
                LapData[i].bestLapNumber = manager.GetByte();

                LapData[i].bestLapSector1Time = manager.GetUnsignedShort();
                LapData[i].bestLapSector2Time = manager.GetUnsignedShort();
                LapData[i].bestLapSector3Time = manager.GetUnsignedShort();

                LapData[i].bestOverallSector1Time = manager.GetUnsignedShort();
                LapData[i].bestOverallSector1LapNumber = manager.GetByte();
                LapData[i].bestOverallSector2Time = manager.GetUnsignedShort();
                LapData[i].bestOverallSector2LapNumber = manager.GetByte();
                LapData[i].bestOverallSector3Time = manager.GetUnsignedShort();
                LapData[i].bestOverallSector3LapNumber = manager.GetByte();

                LapData[i].lapDistance = manager.GetFloat();
                LapData[i].totalDistance = manager.GetFloat();

                LapData[i].safetyCarDelta = manager.GetFloat();
                LapData[i].carPosition = manager.GetByte();
                LapData[i].currentLapNumber = manager.GetByte();
                LapData[i].pitStatus = manager.GetEnumFromByte<PitStatus>();
                LapData[i].currentSector = manager.GetEnumFromByte<LapState>();
                LapData[i].currentLapInvalid = manager.GetBool();
                LapData[i].totalPenalties = manager.GetByte();
                LapData[i].gridPosition = manager.GetByte();
                LapData[i].driverStatus = manager.GetEnumFromByte<DriverStatus>();
                LapData[i].resultStatus = manager.GetEnumFromByte<ResultStatus>();
            }
        }
    }

    /// <summary>
    /// Holds LapData for one driver in the race
    /// </summary>
    public struct LapData
    {
        /// <summary>
        /// This car's last lap time in seconds
        /// </summary>
        public float lastLapTime;
        /// <summary>
        /// This car's Current lap time in seconds
        /// </summary>
        public float currentLapTime;

        /// <summary>
        /// This car's current Sector1 time in millieseconds
        /// </summary>
        public ushort sector1Time;
        /// <summary>
        /// This car's current Sector2 time in millieseconds
        /// </summary>
        public ushort sector2Time;
        /// <summary>
        /// This car's best lap time of the session in seconds
        /// </summary>
        public float bestLapTime;
        /// <summary>
        /// This car's lap number of the fastest lap
        /// </summary>
        public byte bestLapNumber;

        /// <summary>
        /// This car's Best Sector1 time in millieseconds
        /// </summary>
        public ushort bestLapSector1Time;
        /// <summary>
        /// This car's Best Sector2 time in millieseconds
        /// </summary>
        public ushort bestLapSector2Time;
        /// <summary>
        /// This car's Best Sector3 time in millieseconds
        /// </summary>
        public ushort bestLapSector3Time;

        //Must be in this weird order when reading from data :(
        /// <summary>
        /// The fastest car in the session best Sector1 time in millieseconds
        /// </summary>
        public ushort bestOverallSector1Time;
        /// <summary>
        /// The fastest car in the session best Sector1 time, what lap it was set on 
        /// </summary>
        public byte bestOverallSector1LapNumber;
        /// <summary>
        /// The fastest car in the session best Sector2 time in millieseconds
        /// </summary>
        public ushort bestOverallSector2Time;
        /// <summary>
        /// The fastest car in the session best Sector2 time, what lap it was set on 
        /// </summary>
        public byte bestOverallSector2LapNumber;
        /// <summary>
        /// The fastest car in the session best Sector3 time in millieseconds
        /// </summary>
        public ushort bestOverallSector3Time;
        /// <summary>
        /// The fastest car in the session best Sector3 time, what lap it was set on 
        /// </summary>
        public byte bestOverallSector3LapNumber;


        //Distances
        /// <summary>
        /// Distance this car is around the track this lap in metres (possible to me negative if starting behind finish line)
        /// </summary>
        public float lapDistance;
        /// <summary>
        /// Distance this car is around the track in total in metres (possible to me negative if starting behind finish line)
        /// </summary>
        public float totalDistance;

        //Varied
        /// <summary>
        /// SC delta in seconds (this car)
        /// </summary>
        public float safetyCarDelta;
        /// <summary>
        /// Position in race
        /// </summary>
        public byte carPosition;
        /// <summary>
        /// What lap this car is on
        /// </summary>
        public byte currentLapNumber;
        public PitStatus pitStatus;
        /// <summary>
        /// Current sector
        /// </summary>
        public LapState currentSector;
        /// <summary>
        /// Is this current lap invalid? True if invalid
        /// </summary>
        public bool currentLapInvalid;
        /// <summary>
        /// Total amount of penalties for this car in seconds
        /// </summary>
        public byte totalPenalties;
        /// <summary>
        /// The position the vehicle started the race in
        /// </summary>
        public byte gridPosition;
        public DriverStatus driverStatus;
        public ResultStatus resultStatus;
    }
}
