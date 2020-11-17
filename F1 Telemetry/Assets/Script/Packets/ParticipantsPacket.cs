using UnityEngine;
using System.Text;

public class ParticipantsPacket : Packet
{
    static bool test = true;

    protected readonly int PARTICIPANT_DATA_SIZE = 54; //Size in bytes of entire data struct of ParticipantData
    protected readonly int PARTICIPANT_DATA_INDEX = 25; //Start index of first instance of ParticipantData[]
    protected readonly int NAME_AMOUNT_OF_CHARS = 48; //Amount of bytes to make up name in ParticipantData
    protected readonly int PUBLIC_TELEMETRY = 1; //Value if telemetry is public

    public byte NumberOfActiveCars { get; protected set; }
    public ParticipantData[] ParticipantData { get; protected set; }

    public ParticipantsPacket(byte[] data) : base(data) { }

    /// <summary>
    /// Loads in Header data and Participant data, must run before working on data
    /// </summary>
    public override void LoadBytes()
    {
        base.LoadBytes();
        ByteManager manager = new ByteManager(Data, MOVE_PAST_HEADER_INDEX);

        NumberOfActiveCars = manager.GetByte();
        ParticipantData = new ParticipantData[NumberOfActiveCars];

        //Read all instances of ParticipantData[] in the data -> It's all linear so we only need to know
        //Length in bytes of each entry and what index entry 0 has to loop through entire array
        for (int i = 0; i < ParticipantData.Length; i++)
        {
            //Find startindex for current ParticipantData
            int offsetIndex = PARTICIPANT_DATA_INDEX + PARTICIPANT_DATA_SIZE * i;
            manager.SetNewIndex(offsetIndex);

            ParticipantData[i].AIControlled = (ControlledStatus)manager.GetByte();
            ParticipantData[i].driverID = manager.GetByte();
            ParticipantData[i].team = (Team)manager.GetByte();
            ParticipantData[i].raceNumber = manager.GetByte();
            ParticipantData[i].nationality = (Nationality)manager.GetByte();
            ParticipantData[i].name = Encoding.UTF8.GetString(manager.GetBytes(NAME_AMOUNT_OF_CHARS));
            ParticipantData[i].publicTelemetry = manager.GetByte() == PUBLIC_TELEMETRY;
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
}