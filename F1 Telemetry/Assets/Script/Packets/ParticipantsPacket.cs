using UnityEngine;
using System.Text;

/// <summary>
/// This is a list of participants in the race. If controlled by AI -> name is driver name, online, name -> steam 
/// 1 packet every 5 seconds
/// </summary>
public class ParticipantsPacket : Packet
{
    public byte NumberOfActiveCars { get; private set; }
    public ParticipantData[] AllParticipantData { get; private set; }

    public ParticipantsPacket(byte[] data) : base(data) { }

    /// <summary>
    /// Loads in Header data and Participant data, must run before working on data
    /// </summary>
    public override void LoadBytes()
    {
        base.LoadBytes();
        ByteManager manager = new ByteManager(Data, MOVE_PAST_HEADER_INDEX);

        NumberOfActiveCars = manager.GetByte();
        AllParticipantData = new ParticipantData[NumberOfActiveCars];

        int participantDataIndex = manager.CurrentIndex;

        //Read all instances of ParticipantData[] in the data -> It's all linear so we only need to know
        //Length in bytes of each entry and what index entry 0 has to loop through entire array
        for (int i = 0; i < AllParticipantData.Length; i++)
        {
            //Find startindex for current ParticipantData
            int offsetIndex = participantDataIndex + ParticipantData.SIZE * i;
            manager.SetNewIndex(offsetIndex);

            AllParticipantData[i].AIControlled = (ControlledStatus)manager.GetByte();
            AllParticipantData[i].driverID = manager.GetByte();
            AllParticipantData[i].team = (Team)manager.GetByte();
            AllParticipantData[i].raceNumber = manager.GetByte();
            AllParticipantData[i].nationality = (Nationality)manager.GetByte();
            AllParticipantData[i].name = Encoding.UTF8.GetString(manager.GetBytes(ParticipantData.AMOUNT_OF_CHARS_IN_NAME));
            AllParticipantData[i].publicTelemetry = manager.GetByte() == STATEMENT_TRUE;
        }
    }
}

/// <summary>
/// Holds ParticipantData for one driver in the race
/// </summary>
public struct ParticipantData
{
    public ControlledStatus AIControlled;
    public byte driverID;
    public Team team;
    public byte raceNumber;
    public Nationality nationality;
    public string name;
    public bool publicTelemetry;

    /// <summary>
    /// Size in bytes of an instance of ParticipantData in data
    /// </summary>
    public static int SIZE
    {
        get
        {
            return sizeof(byte) * 5 + AMOUNT_OF_CHARS_IN_NAME + sizeof(bool); //Enums are made from only one byte of data
        }
    }

    public static readonly int AMOUNT_OF_CHARS_IN_NAME = 48; //Amount of bytes to make up name in ParticipantData / Packet
}