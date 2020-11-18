using UnityEngine;

/// <summary>
/// The Lap data packet gives details of all the cars in the session.
/// Rate specified in menus (high frequency)
/// </summary>
public class LapDataPacket : Packet
{
    public LapDataPacket(byte[] data) : base(data) { }

    public override void LoadBytes()
    {
        base.LoadBytes();
    }
}

public struct LapData
{
    //Lap data
    public float lastLapTime;     //This car's last lap time in seconds
    public float currentLapTime;  //This car's Current lap time in seconds
                                              
    public ushort sector1Time;  //This car's current Sector1 time in millieseconds
    public ushort sector2Time;  //This car's current Sector2 time in millieseconds
    public float bestLapTime;   //This car's best lap time of the session in seconds
    public byte bestLapNumber;  //This car's lap number of the fastest lap
                                              
    public ushort bestLapSector1Time;  //This car's Best Sector1 time in millieseconds
    public ushort bestLapSector2Time;  //This car's Best Sector2 time in millieseconds
    public ushort bestLapSector3Time;  //This car's Best Sector3 time in millieseconds
                                              
    public ushort bestOverallSector1Time;  //The fastest car in the session best Sector1 time in millieseconds
    public ushort bestOverallSector2Time;  //The fastest car in the session best Sector2 time in millieseconds
    public ushort bestOverallSector3Time;  //The fastest car in the session best Sector3 time in millieseconds

    public byte bestOverallSector1LapNumber;  //The fastest car in the session best Sector1 time, what lap it was set on 
    public byte bestOverallSector2LapNumber;  //The fastest car in the session best Sector2 time, what lap it was set on 
    public byte bestOverallSector3LapNumber;  //The fastest car in the session best Sector3 time, what lap it was set on 

    //Distances
    public float lapDistance;   //Distance this car is around the track this lap in metres (possible to me negative if starting behind finish line)
    public float totalDistance; //Distance this car is around the track in total in metres (possible to me negative if starting behind finish line)

    //Varied
    public float safetyCarDelta;       //SC delta in seconds (this car)
    public byte carPosition;           //Position in race
    public byte currentLapNumber;      //What lap this car is on
    public PitStatus pitStatus;        
    public byte currentSector;         //Current sector (0 -> Sector1, 1 -> Sector2, 2 -> Sector3)
    public bool currentLapInvalid;     //Is this current lap invalid? True if invalid
    public byte totalPenalties;        //Total amount of penalties for this car in seconds
    public byte gridPosition;          //The position the vehicle started the race in
    public DriverStatus driverStatus;  
    public ResultStatus resultStatus;  //Experiment with this one
}
