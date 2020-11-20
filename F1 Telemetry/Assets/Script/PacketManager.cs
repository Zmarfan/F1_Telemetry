using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Lines of code so far: 2095
public class PacketManager : MonoBehaviour
{
    static Queue<byte[]> _dataPackets = new Queue<byte[]>(); //Queue of all packets received since last frame

    /// <summary>
    /// Add a packet to the queue that will be processed next frame
    /// </summary>
    public static void AddPacketData(byte[] data)
    {
        _dataPackets.Enqueue(data);
    }

    private void Update()
    {
        //Handle all the packets that have come in since last frame
        while (_dataPackets.Count > 0)
            ReadPacket(_dataPackets.Dequeue());
    }

    /// <summary>
    /// Identifies packet type by data header and handles it ackoringly -> Read data from header and transmits it further
    /// </summary>
    void ReadPacket(byte[] packetData)
    {
        Packet packet = GetPacketType(packetData);
        packet.LoadBytes();
        //HandlePacket(packet);
    }

    void HandlePacket(Packet packet)
    {
        switch ((PacketType)packet.PacketID)
        {
            case PacketType.MOTION:
                {
                    Participants.SetMotionPacket((MotionPacket)packet);
                    break;
                }
            case PacketType.SESSION:
                break;
            case PacketType.LAP_DATA:
                {
                    Participants.SetLapData((LapDataPacket)packet);
                    break;
                }
            case PacketType.EVENT:
                break;
            case PacketType.PARTICIPANTS:
                {
                    Participants.SetParticipantsPacket((ParticipantsPacket)packet);
                    break;
                }
            case PacketType.CAR_SETUPS:
                {
                    Participants.SetCarSetupData((CarSetupPacket)packet);
                    break;
                }
            case PacketType.CAR_TELEMETRY:
                {
                    Participants.SetTelemetryData((CarTelemetryPacket)packet);
                    break;
                }
            case PacketType.CAR_STATUS:
                {
                    Participants.SetCarStatusData((CarStatusPacket)packet);
                    break;
                }
            case PacketType.FINAL_CLASSIFICATION:
                break;
            case PacketType.LOBBY_INFO:
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Id what type of packet it is and convert it to that typ of package
    /// </summary>
    Packet GetPacketType(byte[] data)
    {
        //Id what type of packet this is
        PacketType packetType = Packet.GetPacketType(data);

        switch (packetType)
        {
            case (PacketType.MOTION):               { return new MotionPacket(data); }
            case (PacketType.SESSION):              { return new SessionPacket(data); }
            case (PacketType.LAP_DATA):             { return new LapDataPacket(data); }
            case (PacketType.PARTICIPANTS):         { return new ParticipantsPacket(data); }
            case (PacketType.CAR_SETUPS):           { return new CarSetupPacket(data); }
            case (PacketType.CAR_TELEMETRY):        { return new CarTelemetryPacket(data); }
            case (PacketType.CAR_STATUS):           { return new CarStatusPacket(data); }
            case (PacketType.FINAL_CLASSIFICATION): { return new FinalClassificationPacket(data); }
            case (PacketType.LOBBY_INFO):           { return new LobbyInfoPacket(data); }
            case (PacketType.EVENT):
                {
                    //Id what type of event packet this is
                    EventType eventType = EventPacket.EventPacketType(data);

                    switch (eventType)
                    {
                        //return base class EventPacket if nothing more needs to be known than the event occured!
                        case EventType.Session_Started:        { return new EventPacket(data); } 
                        case EventType.Session_Ended:          { return new EventPacket(data); } 
                        case EventType.DRS_Enabled:            { return new EventPacket(data); } 
                        case EventType.DRS_Disabled:           { return new EventPacket(data); } 
                        case EventType.Chequered_Flag:         { return new EventPacket(data); } 

                        case EventType.Fastest_Lap:            { return new FastestLapEventPacket(data); }
                        case EventType.Retirement:             { return new RetirementEventPacket(data); }
                        case EventType.Team_Mate_In_Pits:      { return new TeamMateInPitsEventPacket(data); }
                        case EventType.Race_Winner:            { return new RaceWinnerEventPacket(data); }
                        case EventType.Penalty_Issued:         { return new PenaltyEventPacket(data); }
                        case EventType.Speed_Trap_Triggered:   { return new SpeedTrapEventPacket(data); }
                        default: { throw new System.Exception("There exist no such eventype: " + eventType); }
                    }
                }
            default: { throw new System.Exception("There exist no such packet type: " + packetType); }
        }
    }
}
