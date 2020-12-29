namespace F1_Data_Management
{
    /// <summary>
    /// Holds session data read from Session packets.
    /// </summary>
    public class SessionManager
    {
        public bool ReadyToReadFrom { get; private set; } = false;
        Session SessionData { get; set; }
        //Used to send out event when session has changed
        EventManager _eventManager;

        public SessionManager(EventManager eventManager)
        {
            _eventManager = eventManager;
        }

        /// <summary>
        /// Returns a copy of latest saved Session Data
        /// </summary>
        public Session GetSessionDataCopy(out bool status)
        {
            status = ReadyToReadFrom;
            Session copy = new Session();
            //Copies all by value data
            copy = SessionData;
            //If it's not ready to be read -> return junk
            if (!status)
                return copy;

            //Hard copies all by reference data
            MarshalZone[] copyMarshalZone = new MarshalZone[SessionData.MarshalZones.Length];
            for (int i = 0; i < copyMarshalZone.Length; i++)
            {
                copyMarshalZone[i].zoneFlag = SessionData.MarshalZones[i].zoneFlag;
                copyMarshalZone[i].zoneStart = SessionData.MarshalZones[i].zoneStart;
            }
            copy.MarshalZones = copyMarshalZone;

            WeatherForecastSample[] copyWeatherForecastCopy = new WeatherForecastSample[SessionData.WeatherForecastSamples.Length];
            for (int i = 0; i < copyWeatherForecastCopy.Length; i++)
            {
                copyWeatherForecastCopy[i].airTemperature = SessionData.WeatherForecastSamples[i].airTemperature;
                copyWeatherForecastCopy[i].sessionType = SessionData.WeatherForecastSamples[i].sessionType;
                copyWeatherForecastCopy[i].timeOffset = SessionData.WeatherForecastSamples[i].timeOffset;
                copyWeatherForecastCopy[i].trackTemperature = SessionData.WeatherForecastSamples[i].trackTemperature;
                copyWeatherForecastCopy[i].weather = SessionData.WeatherForecastSamples[i].weather;
            }
            copy.WeatherForecastSamples = copyWeatherForecastCopy;

            return copy;
        }

        /// <summary>
        /// Clears out saved data.
        /// </summary>
        public void Clear()
        {
            SessionData = new Session();
            ReadyToReadFrom = false;
        }

        /// <summary>
        /// Copy data from packet to interface for user to be able to read safely.
        /// </summary>
        public void UpdateSessionData(SessionPacket sessionPacket)
        {
            ReadyToReadFrom = true;
            //If the session has changed since last update -> invoke event
            SessionType lastSessionType = SessionData.SessionType;
            if (lastSessionType != sessionPacket.SessionType)
                _eventManager.InvokeSessionChangeEvent(sessionPacket.SessionType);

            Session newSessionData = new Session();
            newSessionData.Weather = sessionPacket.Weather;
            newSessionData.TrackTemperature = sessionPacket.TrackTemperature;
            newSessionData.AirTemperature = sessionPacket.AirTemperature;
            newSessionData.TotalLaps = sessionPacket.TotalLaps;
            newSessionData.TrackLength = sessionPacket.TrackLength;
            newSessionData.SessionType = sessionPacket.SessionType;
            newSessionData.Track = sessionPacket.Track;
            newSessionData.Formula = sessionPacket.Formula;
            newSessionData.SessionTimeLeft = sessionPacket.SessionTimeLeft;
            newSessionData.SessionDuration = sessionPacket.SessionDuration;
            newSessionData.PitSpeedLimit = sessionPacket.PitSpeedLimit;
            newSessionData.GamePaused = sessionPacket.GamePaused;
            newSessionData.IsSpectating = sessionPacket.IsSpectating;
            newSessionData.SpectatorCarIndex = sessionPacket.SpectatorCarIndex;
            newSessionData.SliProNativeSupport = sessionPacket.SliProNativeSupport;
            newSessionData.SafetyCarStatus = sessionPacket.SafetyCarStatus;
            newSessionData.IsOnline = sessionPacket.IsOnline;
            newSessionData.MarshalZones = sessionPacket.MarshalZones;
            newSessionData.WeatherForecastSamples = sessionPacket.WeatherForecastSamples;

            SessionData = newSessionData;
        }
    }

    public struct Session
    {
        /// <summary>
        /// Current weather right now
        /// </summary>
        public Weather Weather { get; set; }
        /// <summary>
        /// Current track temperature in celsius
        /// </summary>
        public sbyte TrackTemperature { get; set; }
        /// <summary>
        /// Current air temperature in celsius
        /// </summary>
        public sbyte AirTemperature { get; set; }
        public byte TotalLaps { get; set; }
        /// <summary>
        /// Track length in metres
        /// </summary>
        public ushort TrackLength { get; set; }
        public SessionType SessionType { get; set; }
        public Track Track { get; set; }
        /// <summary>
        /// What type of cars in the session
        /// </summary>
        public Formula Formula { get; set; }
        public ushort SessionTimeLeft { get; set; }
        public ushort SessionDuration { get; set; }
        /// <summary>
        /// In km/h
        /// </summary>
        public byte PitSpeedLimit { get; set; }
        public bool GamePaused { get; set; }
        public bool IsSpectating { get; set; }
        /// <summary>
        /// 255 if not specating
        /// </summary>
        public byte SpectatorCarIndex { get; set; }
        /// <summary>
        /// Whether or not SLI Pro is supported
        /// </summary>
        public bool SliProNativeSupport { get; set; }
        public SafetyCarStatus SafetyCarStatus { get; set; }
        public bool IsOnline { get; set; }
        /// <summary>
        /// Holds info about marshalZones. Zone Position and flag status.
        /// </summary>
        public MarshalZone[] MarshalZones { get; set; }
        /// <summary>
        /// Data about weather for a specific timeframe and session
        /// </summary>
        public WeatherForecastSample[] WeatherForecastSamples { get; set; }
    }
}

