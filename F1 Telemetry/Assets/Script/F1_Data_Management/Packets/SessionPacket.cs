namespace F1_Data_Management
{
    /// <summary>
    /// The session packet includes details about the current session in progress. 
    /// 2 packets per second.
    /// </summary>
    public class SessionPacket : Packet
    {
        /// <summary>
        /// Current weather right now
        /// </summary>
        public Weather Weather { get; private set; } 
        public sbyte TrackTemperature { get; private set; }
        public sbyte AirTemperature { get; private set; }
        public byte TotalLaps { get; private set; }
        /// <summary>
        /// Track length in metres
        /// </summary>
        public ushort TrackLength { get; private set; }
        public SessionType SessionType { get; private set; }
        public Track Track { get; private set; }
        /// <summary>
        /// What type of cars in the session
        /// </summary>
        public Formula Formula { get; private set; }
        public ushort SessionTimeLeft { get; private set; }
        public ushort SessionDuration { get; private set; }
        /// <summary>
        /// In km/h
        /// </summary>
        public byte PitSpeedLimit { get; private set; }
        public bool GamePaused { get; private set; }
        public bool IsSpectating { get; private set; }
        /// <summary>
        /// 255 if not specating
        /// </summary>
        public byte SpectatorCarIndex { get; private set; }
        /// <summary>
        /// Whether or not SLI Pro is supported
        /// </summary>
        public bool SliProNativeSupport { get; private set; }
        public byte NumberOfMarshalZones { get; private set; }
        /// <summary>
        /// Info about marshal zones (flag and position in lap)
        /// </summary>
        public MarshalZone[] MarshalZones { get; private set; }
        public SafetyCarStatus SafetyCarStatus { get; private set; }
        public bool IsOnline { get; private set; }
        public byte NumberWeatherForeCastSamples { get; private set; }
        public WeatherForecastSample[] WeatherForecastSamples { get; private set; }


        public SessionPacket(byte[] data) : base(data) { }

        public override void LoadBytes()
        {
            base.LoadBytes();
            ByteManager manager = new ByteManager(Data, MOVE_PAST_HEADER_INDEX, "Session packet");

            Weather = manager.GetEnumFromByte<Weather>();
            TrackTemperature = manager.GetSignedByte();
            AirTemperature = manager.GetSignedByte();
            TotalLaps = manager.GetByte();
            TrackLength = manager.GetUnsignedShort();
            SessionType = manager.GetEnumFromByte<SessionType>();
            Track = manager.GetEnumFromByte<Track>();
            Formula = manager.GetEnumFromByte<Formula>();
            SessionTimeLeft = manager.GetUnsignedShort();
            SessionDuration = manager.GetUnsignedShort();
            PitSpeedLimit = manager.GetByte();
            GamePaused = manager.GetBool();
            IsSpectating = manager.GetBool();
            SpectatorCarIndex = manager.GetByte();
            SliProNativeSupport = manager.GetBool();
            NumberOfMarshalZones = manager.GetByte();

            MarshalZones = new MarshalZone[NumberOfMarshalZones];

            //Read all instances of MarshalZone[] in the data -> It's all linear
            for (int i = 0; i < MarshalZones.Length; i++)
            {
                MarshalZones[i].zoneStart = manager.GetFloat();
                MarshalZones[i].zoneFlag = manager.GetEnumFromSignedByte<Flag>();
            }
            //manager will now have moved past struct array

            SafetyCarStatus = (SafetyCarStatus)manager.GetByte();
            IsOnline = manager.GetBool();
            NumberWeatherForeCastSamples = manager.GetByte();

            WeatherForecastSamples = new WeatherForecastSample[NumberWeatherForeCastSamples];

            //Read all instances of WeatherForecastSamples[] in the data -> It's all linear
            for (int i = 0; i < WeatherForecastSamples.Length; i++)
            {
                WeatherForecastSamples[i].sessionType = manager.GetEnumFromByte<SessionType>();
                WeatherForecastSamples[i].timeOffset = manager.GetByte();
                WeatherForecastSamples[i].weather = manager.GetEnumFromByte<Weather>();
                WeatherForecastSamples[i].trackTemperature = manager.GetSignedByte();
                WeatherForecastSamples[i].airTemperature = manager.GetSignedByte();
            }
        }
    }

    /// <summary>
    /// Status of different marshal zones around the track
    /// </summary>
    public struct MarshalZone
    {
        /// <summary>
        /// Fraction (0..1) of way through the lap the marshal zone starts 
        /// </summary>
        public float zoneStart;
        public Flag zoneFlag;
    }

    /// <summary>
    /// Variables about weather for a specific timeframe and session
    /// </summary>
    public struct WeatherForecastSample
    {
        public SessionType sessionType;
        /// <summary>
        /// Time in minutes the forecast is for
        /// </summary>
        public byte timeOffset;
        public Weather weather;
        public sbyte trackTemperature;
        public sbyte airTemperature;
    }
}