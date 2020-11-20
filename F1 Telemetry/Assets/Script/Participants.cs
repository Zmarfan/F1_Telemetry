using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds participant data to be reached from entire project
/// </summary>
public static class Participants
{
    static DriverData _data = new DriverData();

    public static DriverData Data { get { return _data; } }

    /// <summary>
    /// Called every 5s from PacketManager to refresh data
    /// </summary>
    public static void SetParticipantsPacket(ParticipantsPacket data)
    {
        _data.ParticipantData = data.AllParticipantData;
    }

    /// <summary>
    /// Called 2 times per second from PacketManager to refresh data
    /// </summary>
    public static void SetMotionPacket(MotionPacket data)
    {
        _data.MotionData = data.AllCarMotionData;
    }

    /// <summary>
    /// Called often from PacketManager to refresh data
    /// </summary>
    public static void SetLapData(LapDataPacket data)
    {
        _data.LapData = data.LapData;
    }

    /// <summary>
    /// Called often from PacketManager to refresh data
    /// </summary>
    public static void SetTelemetryData(CarTelemetryPacket data)
    {
        _data.TelemetryData = data.AllCarTelemetryData;
    }

    /// <summary>
    /// Called often from PacketManager to refresh data
    /// </summary>
    public static void SetCarStatusData(CarStatusPacket data)
    {
        _data.StatusData = data.AllCarStatusData;
    }

    /// <summary>
    /// Called 2 times every second from PacketManager to refresh data
    /// </summary>
    public static void SetCarSetupData(CarSetupPacket data)
    {
        _data.CarSetup = data.AllCarSetups;
    }
}

/// <summary>
/// Holds data about every driver
/// </summary>
public struct DriverData
{
    public ParticipantData[] ParticipantData { get; set; }
    public CarMotionData[] MotionData { get; set; }
    public LapData[] LapData { get; set; }
    public CarTelemetryData[] TelemetryData { get; set; }
    public CarStatusData[] StatusData { get; set; }
    public CarSetup[] CarSetup { get; set; }
}
