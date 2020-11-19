using UnityEngine;

/// <summary>
/// This packet details the final classification at the end of the race
/// Sent once at end of a race
/// </summary>
public class FinalClassificationPacket : Packet
{
    public byte NumberOfCars { get; private set; }
    public FinalClassificationData[] AllFinalClassificationData { get; private set; }

    public FinalClassificationPacket(byte[] data) : base(data) { }

    public override void LoadBytes()
    {
        base.LoadBytes();

        ByteManager manager = new ByteManager(Data, MOVE_PAST_HEADER_INDEX, "Final Classification Packet");

        NumberOfCars = manager.GetByte();
        AllFinalClassificationData = new FinalClassificationData[NumberOfCars];

        //Read all instances of ParticipantData[] in the data -> It's all linear
        for (int i = 0; i < AllFinalClassificationData.Length; i++)
        {
            AllFinalClassificationData[i].position = manager.GetByte();
            AllFinalClassificationData[i].numberOfLaps = manager.GetByte();
            AllFinalClassificationData[i].gridPosition = manager.GetByte();
            AllFinalClassificationData[i].points = manager.GetByte();
            AllFinalClassificationData[i].numberOfPitStops = manager.GetByte();
            AllFinalClassificationData[i].resultStatus = manager.GetEnumFromByte<ResultStatus>();
            AllFinalClassificationData[i].bestLapTime = manager.GetFloat();
            AllFinalClassificationData[i].totalRaceTime = manager.GetDouble();
            AllFinalClassificationData[i].penaltiesTime = manager.GetByte();
            AllFinalClassificationData[i].numberOfPenalties = manager.GetByte();
            AllFinalClassificationData[i].numberOfTyreStints = manager.GetByte();
            AllFinalClassificationData[i].tyreStintsActual = manager.GetEnumArrayFromBytes<ActualTyreCompound>(Wheel.WHEEL_COUNT);
            AllFinalClassificationData[i].tyreStintsVisual = manager.GetEnumArrayFromBytes<VisualTyreCompound>(Wheel.WHEEL_COUNT);
        }
    }
}

/// <summary>
/// Holds FinalClassificationData for one driver in the race
/// </summary>
public struct FinalClassificationData
{
    public byte position;                          //Finishing position
    public byte numberOfLaps;                      //Number of completed laps
    public byte gridPosition;                      //Starting position
    public byte points;                            //points scored
    public byte numberOfPitStops;                  
    public ResultStatus resultStatus;              
    public float bestLapTime;                      //Best lap time in seconds
    public double totalRaceTime;                   //Total race time in seconds WITHOUT penalties
    public byte penaltiesTime;                     //All penalties added together in seconds
    public byte numberOfPenalties;
    public byte numberOfTyreStints;                //vvvv Index for tyre compounds vvvv
    public ActualTyreCompound[] tyreStintsActual;  //Maximum of 8 -> shows C1, C2, C3, C4, C5
    public VisualTyreCompound[] tyreStintsVisual;  //Maximum of 8 -> shows soft, medium, hard
}
