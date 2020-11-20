using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds participant data to be reached from entire project
/// </summary>
public static class Participants
{
    //RESET ALL VALUES HERE WHEN ENDING SESSION

    static DriverData _data = new DriverData();
    static Dictionary<int, bool> _validVehicleIndexChecker; //All the cars with true have valid values in their data, rest in junk

    public static DriverData Data { get { return _data; } } //All data for every single car
    public static int ActiveDrivers { get; private set; }   //Amount of drivers actually competing

    /// <summary>
    /// Only read data when it's actually there
    /// </summary>
    public static bool ReadyToReadFrom
    {
        get
        {
            return _data.LapData != null && _data.MotionData != null && _data.ParticipantData != null && _data.StatusData != null && _data.TelemetryData != null;
        }
    }

    public static bool ValidIndex(int index)
    {
        //Init vehicleIndexChecker -> done once / if new people join lobby
        if (_validVehicleIndexChecker == null || !_validVehicleIndexChecker.ContainsKey(index))
            InitValidVehicleIndexChecker();

        //Valid index return true, invalid false
        return _validVehicleIndexChecker[index];
    }

    /// <summary>
    /// Called every 5s from PacketManager to refresh data
    /// </summary>
    public static void SetParticipantsPacket(ParticipantsPacket data)
    {
        ActiveDrivers = data.NumberOfActiveCars;
        _data.ParticipantData = data.AllParticipantData;
        //Every 5s the dictionary will refresh to make sure lobby is correct
        InitValidVehicleIndexChecker();
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

    /// <summary>
    /// Reset dictionary so correct drivers are in there, called every 5s for safety
    /// </summary>
    static void InitValidVehicleIndexChecker()
    {
        _validVehicleIndexChecker = new Dictionary<int, bool>();

        for (int i = 0; i < _data.LapData.Length; i++)
        {
            //The data can be considered junk if ResultStatus in LapData is "Inactive" or "Invalid"
            if (!(_data.LapData[i].resultStatus == ResultStatus.Inactive || _data.LapData[i].resultStatus == ResultStatus.Invalid))
                _validVehicleIndexChecker.Add(i, true);
            else
                _validVehicleIndexChecker.Add(i, false);
        }
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
