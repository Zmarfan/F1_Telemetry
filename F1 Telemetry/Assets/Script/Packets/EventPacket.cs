using System.Text;
using UnityEngine;

/// <summary>
/// This packet gives details of events that happen during the course of a session.
/// Packet sent when event occurs
/// </summary>
public class EventPacket : Packet
{
    public EventType EventType  { get; private set; }

    protected static readonly int EVENT_TYPE_BYTE_SIZE = 4;
    protected static readonly int MOVE_PAST_EVENT_HEADER = 28;

    #region Event String Codes
    public const string SESSION_STARTED =      "SSTA";
    public const string SESSION_ENDED =        "SEND";
    public const string FASTEST_LAP =          "FTLP";
    public const string RETIREMENT =           "RTMT";
    public const string DRS_ENABLED =          "DRSE";
    public const string DRS_DISABLED =         "DRSD";
    public const string TEAM_MATE_IN_PITS =    "TMPT";
    public const string CHEQUERED_FLAG =       "CHQF";
    public const string RACE_WINNER =          "RCWN";
    public const string PENALTY_ISSUED =       "PENA";
    public const string SPEED_TRAP_TRIGGERED = "SPTP";
    #endregion

    public EventPacket(byte[] data) : base(data) { }

    public override void LoadBytes()
    {
        base.LoadBytes();
        //This has already been done with this data but is done again to keep structure across all packets
        EventType = EventPacketType(Data);
    }

    /// <summary>
    /// Identifies what type of even packet this is and returns that type
    /// </summary>
    public static EventType EventPacketType(byte[] data)
    {
        ByteManager manager = new ByteManager(data, MOVE_PAST_HEADER_INDEX, "Eventpacket");

        string eventStringCode = manager.GetString(EVENT_TYPE_BYTE_SIZE);

        switch (eventStringCode)
        {
            case (SESSION_STARTED):      { return EventType.Session_Started; }
            case (SESSION_ENDED):        { return EventType.Session_Ended; }
            case (FASTEST_LAP):          { return EventType.Fastest_Lap; }
            case (RETIREMENT):           { return EventType.Retirement; }
            case (DRS_ENABLED):          { return EventType.DRS_Enabled; }
            case (DRS_DISABLED):         { return EventType.DRS_Disabled; }
            case (TEAM_MATE_IN_PITS):    { return EventType.Team_Mate_In_Pits; }
            case (CHEQUERED_FLAG):       { return EventType.Chequered_Flag; }
            case (RACE_WINNER):          { return EventType.Race_Winner; }
            case (PENALTY_ISSUED):       { return EventType.Penalty_Issued; }
            case (SPEED_TRAP_TRIGGERED): { return EventType.Speed_Trap_Triggered; }
            default: throw new System.Exception("There exist no string code for: " + eventStringCode);
        }
    }
}