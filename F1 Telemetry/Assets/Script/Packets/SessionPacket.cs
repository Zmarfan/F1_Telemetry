using UnityEngine;
using System;

public class SessionPacket : Packet
{
    public static int MARSHALZONE_DATA_INDEX = 43;  //Start index of first instance of MarshalZone[]
    public static int MARSHALZONE_DATA_SIZE = 5;   //Size in bytes of entire data struct of MarshalZone
    public static int WEATHER_FORECAST_SAMPLE_DATA_INDEX = 131;  //Start index of first instance of WeatherForecastSamples[]
    public static int WEATHER_FORECAST_SAMPLE_DATA_SIZE = 5;   //Size in bytes of entire data struct of WeatherForecastSample
    public readonly int STATEMENT_TRUE = 1;

    public Weather Weather { get; protected set; }                                   //Current weather right now
    public sbyte TrackTemperature { get; protected set; }
    public sbyte AirTemperature { get; protected set; }
    public byte TotalLaps { get; protected set; }                                    //Total laps in the race
    public ushort TrackLength { get; protected set; }                                //Track length in metres
    public SessionType SessionType { get; protected set; }                           //Q1/Race/Time-trial etc
    public Track Track { get; protected set; }                                       
    public Formula Formula { get; protected set; }                                   //What type of cars in the session
    public ushort SessionTimeLeft { get; protected set; }                            
    public ushort SessionDuration { get; protected set; }                            
    public byte PitSpeedLimit { get; protected set; }                                //In km/h
    public bool GamePaused { get; protected set; }                                   //Represented by a "byte" guessing 0 -> not paused, 1 -> paused 
    public bool IsSpectating { get; protected set; }                                 //Represented by a "byte" guessing 0 -> not spectating, 1 -> spectating
    public byte SpectatorCarIndex { get; protected set; }                            //255 if not spectating
    public bool SliProNativeSupport { get; protected set; }                          //Whether or not SLI Pro is supported, Represented by a "byte" guessing 0 -> not supported, 1 -> supported
    public byte NumberOfMarshalZones { get; protected set; }                         
    public MarshalZone[] MarshalZones { get; protected set; }                        //Info about marshal zones (flag and position in lap)
    public SafetyCarStatus SafetyCarStatus { get; protected set; }                   //Is SC out and what type?
    public bool IsOnline { get; protected set; }                                     
    public byte NumberWeatherForeCastSamples { get; protected set; }                 //Number of weather samples to follow
    public WeatherForecastSample[] WeatherForecastSamples { get; protected set; }


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

        //Read all instances of MarshalZone[] in the data -> It's all linear so we only need to know
        //Length in bytes of each entry and what index entry 0 has to loop through entire array
        for (int i = 0; i < MarshalZones.Length; i++)
        {
            //Find startindex for current ParticipantData
            int offsetIndex = MARSHALZONE_DATA_INDEX + MARSHALZONE_DATA_SIZE * i;
            manager.SetNewIndex(offsetIndex);

            MarshalZones[i].zoneStart = BitConverter.ToSingle(manager.GetBytes(sizeof(float)), 0);
            MarshalZones[i].zoneFlag = (ZoneFlag)((sbyte)manager.GetByte());  //Needs to be converted to sbyte first since -1 is a possible value
        }
        //manager will now have moved past struct array

        SafetyCarStatus = (SafetyCarStatus)manager.GetByte();
        IsOnline = manager.GetByte() == STATEMENT_TRUE;
        NumberWeatherForeCastSamples = manager.GetByte();

        WeatherForecastSamples = new WeatherForecastSample[NumberWeatherForeCastSamples];

        //Read all instances of WeatherForecastSamples[] in the data -> It's all linear so we only need to know
        //Length in bytes of each entry and what index entry 0 has to loop through entire array
        for (int i = 0; i < WeatherForecastSamples.Length; i++)
        {
            //Find startindex for current ParticipantData
            int offsetIndex = WEATHER_FORECAST_SAMPLE_DATA_INDEX + WEATHER_FORECAST_SAMPLE_DATA_SIZE * i;
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