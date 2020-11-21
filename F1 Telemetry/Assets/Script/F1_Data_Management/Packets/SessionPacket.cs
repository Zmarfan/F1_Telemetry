namespace F1_Data_Management
{
    /// <summary>
    /// The session packet includes details about the current session in progress. 
    /// 2 packets per second.
    /// </summary>
    public class SessionPacket : Packet
    {
        public Weather Weather { get; private set; }                                   //Current weather right now
        public sbyte TrackTemperature { get; private set; }
        public sbyte AirTemperature { get; private set; }
        public byte TotalLaps { get; private set; }                                    //Total laps in the race
        public ushort TrackLength { get; private set; }                                //Track length in metres
        public SessionType SessionType { get; private set; }                           //Q1/Race/Time-trial etc
        public Track Track { get; private set; }
        public Formula Formula { get; private set; }                                   //What type of cars in the session
        public ushort SessionTimeLeft { get; private set; }
        public ushort SessionDuration { get; private set; }
        public byte PitSpeedLimit { get; private set; }                                //In km/h
        public bool GamePaused { get; private set; }                                   //Represented by a "byte" guessing 0 -> not paused, 1 -> paused 
        public bool IsSpectating { get; private set; }                                 //Represented by a "byte" guessing 0 -> not spectating, 1 -> spectating
        public byte SpectatorCarIndex { get; private set; }                            //255 if not spectating
        public bool SliProNativeSupport { get; private set; }                          //Whether or not SLI Pro is supported, Represented by a "byte" guessing 0 -> not supported, 1 -> supported
        public byte NumberOfMarshalZones { get; private set; }
        public MarshalZone[] MarshalZones { get; private set; }                        //Info about marshal zones (flag and position in lap)
        public SafetyCarStatus SafetyCarStatus { get; private set; }                   //Is SC out and what type?
        public bool IsOnline { get; private set; }
        public byte NumberWeatherForeCastSamples { get; private set; }                 //Number of weather samples to follow
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
        public float zoneStart;   //Fraction (0..1) of way through the lap the marshal zone starts 
        public Flag zoneFlag; //Flag status in zone at the moment
    }

    /// <summary>
    /// Variables about weather for a specific timeframe and session
    /// </summary>
    public struct WeatherForecastSample
    {
        public SessionType sessionType;
        public byte timeOffset;         //Time in minutes the forecast is for
        public Weather weather;
        public sbyte trackTemperature;
        public sbyte airTemperature;
    }
}