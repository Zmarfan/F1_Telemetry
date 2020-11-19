using UnityEngine;

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

    public override void LoadBytes()
    {
        base.LoadBytes();

        ByteManager manager = new ByteManager(Data, MOVE_PAST_HEADER_INDEX, "Lap Data Packet");

        LapData = new LapData[MAX_AMOUNT_OF_CARS];

        //This will loop and assign as if there was 22 cars in the race!
        //If there are less cars than 22, these instances will be filled with junk values!
        for (int i = 0; i < MAX_AMOUNT_OF_CARS; i++)
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
            LapData[i].pitStatus = (PitStatus)manager.GetByte();
            LapData[i].currentSector = manager.GetByte();
            LapData[i].currentLapInvalid = manager.GetBool();
            LapData[i].totalPenalties = manager.GetByte();
            LapData[i].gridPosition = manager.GetByte();
            LapData[i].driverStatus = (DriverStatus)manager.GetByte();
            LapData[i].resultStatus = (ResultStatus)manager.GetByte();
        }
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
                                        
    //Must be in this weird order when reading from data :(
    public ushort bestOverallSector1Time;  //The fastest car in the session best Sector1 time in millieseconds
    public byte bestOverallSector1LapNumber;  //The fastest car in the session best Sector1 time, what lap it was set on 
    public ushort bestOverallSector2Time;  //The fastest car in the session best Sector2 time in millieseconds
    public byte bestOverallSector2LapNumber;  //The fastest car in the session best Sector2 time, what lap it was set on 
    public ushort bestOverallSector3Time;  //The fastest car in the session best Sector3 time in millieseconds
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
