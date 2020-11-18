using UnityEngine;
using System;

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
        ByteManager manager = new ByteManager(Data, MOVE_PAST_HEADER_INDEX);

        Weather = (Weather)manager.GetByte();
        TrackTemperature = (sbyte)manager.GetByte();
        AirTemperature = (sbyte)manager.GetByte();
        TotalLaps = manager.GetByte();
        TrackLength = BitConverter.ToUInt16(manager.GetBytes(sizeof(ushort)), 0);
        SessionType = (SessionType)manager.GetByte();
        Track = (Track)((sbyte)manager.GetByte()); //Needs to be converted to sbyte first since -1 is a possible value
        Formula = (Formula)manager.GetByte();
        SessionTimeLeft = BitConverter.ToUInt16(manager.GetBytes(sizeof(ushort)), 0);
        SessionDuration = BitConverter.ToUInt16(manager.GetBytes(sizeof(ushort)), 0);
        PitSpeedLimit = manager.GetByte();
        GamePaused = manager.GetByte() == STATEMENT_TRUE;
        IsSpectating = manager.GetByte() == STATEMENT_TRUE;
        SpectatorCarIndex = manager.GetByte();
        SliProNativeSupport = manager.GetByte() == STATEMENT_TRUE;
        NumberOfMarshalZones = manager.GetByte();

        MarshalZones = new MarshalZone[NumberOfMarshalZones];

        int MarshalZoneDataIndex = manager.CurrentIndex;

        //Read all instances of MarshalZone[] in the data -> It's all linear so we only need to know
        //Length in bytes of each entry and what index entry 0 has to loop through entire array
        for (int i = 0; i < MarshalZones.Length; i++)
        {
            //Find startindex for current ParticipantData
            int offsetIndex = MarshalZoneDataIndex + MarshalZone.SIZE * i;
            manager.SetNewIndex(offsetIndex);

            MarshalZones[i].zoneStart = BitConverter.ToSingle(manager.GetBytes(sizeof(float)), 0);
            MarshalZones[i].zoneFlag = (ZoneFlag)((sbyte)manager.GetByte());  //Needs to be converted to sbyte first since -1 is a possible value
        }
        //manager will now have moved past struct array

        SafetyCarStatus = (SafetyCarStatus)manager.GetByte();
        IsOnline = manager.GetByte() == STATEMENT_TRUE;
        NumberWeatherForeCastSamples = manager.GetByte();

        WeatherForecastSamples = new WeatherForecastSample[NumberWeatherForeCastSamples];

        int WeatherForecastSampleDataIndex = manager.CurrentIndex;

        //Read all instances of WeatherForecastSamples[] in the data -> It's all linear so we only need to know
        //Length in bytes of each entry and what index entry 0 has to loop through entire array
        for (int i = 0; i < WeatherForecastSamples.Length; i++)
        {
            //Find startindex for current ParticipantData
            int offsetIndex = WeatherForecastSampleDataIndex + WeatherForecastSample.SIZE * i;
            manager.SetNewIndex(offsetIndex);

            WeatherForecastSamples[i].sessionType = (SessionType)manager.GetByte();
            WeatherForecastSamples[i].timeOffset = manager.GetByte();
            WeatherForecastSamples[i].weather = (Weather)manager.GetByte();
            WeatherForecastSamples[i].trackTemperature = (sbyte)manager.GetByte();
            WeatherForecastSamples[i].airTemperature = (sbyte)manager.GetByte();
        }
    }
}

/// <summary>
/// Status of different marshal zones around the track
/// </summary>
public struct MarshalZone
{
    public float zoneStart;   //Fraction (0..1) of way through the lap the marshal zone starts 
    public ZoneFlag zoneFlag; //Flag status in zone at the moment

    /// <summary>
    /// Size in bytes of an instance of MarshalZone in data
    /// </summary>
    public static int SIZE
    {
        get
        {
            return sizeof(float) + sizeof(byte); //Enums are made from only one byte of data
        }
    }
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

    /// <summary>
    /// Size in bytes of an instance of WeatherForecastSample in data
    /// </summary>
    public static int SIZE
    {
        get
        {
            return sizeof(byte) * 5; //Enums are made from only one byte of data (sbyte and byte are both 1 byte big)
        }
    }
}