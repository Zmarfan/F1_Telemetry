using System.Collections.Generic;

namespace F1_Data_Management
{
    public class PacketManager
    {
        Participants _participants;
        EventManager _eventManager;

        Queue<byte[]> _dataPackets = new Queue<byte[]>(); //Queue of all packets received since last frame

        public PacketManager(Participants participants, EventManager eventManager)
        {
            _participants = participants;
            _eventManager = eventManager;
        }

        /// <summary>
        /// Add a packet to the queue that will be processed next frame
        /// </summary>
        public void AddPacketData(byte[] data)
        {
            _dataPackets.Enqueue(data);
        }

        /// <summary>
        /// Empty queue of packets and resets all data storage.
        /// </summary>
        public void Reset()
        {
            _dataPackets.Clear();
        }

        /// <summary>
        /// Reads and handles all collectedPackets since this function was last called. UdpReceiver must be activated to have packets receive here.
        /// </summary>
        public void ReadCollectedPackets()
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
            HandlePacket(packet);
        }

        void HandlePacket(Packet packet)
        {
            switch ((PacketType)packet.PacketID)
            {
                case PacketType.MOTION: { _participants.SetMotionPacket((MotionPacket)packet); break; }
                case PacketType.SESSION: { break; }
                case PacketType.LAP_DATA: { _participants.SetLapData((LapDataPacket)packet); break; }
                case PacketType.PARTICIPANTS: { _participants.SetParticipantsPacket((ParticipantsPacket)packet); break; }
                case PacketType.CAR_SETUPS: { _participants.SetCarSetupData((CarSetupPacket)packet); break; }
                case PacketType.CAR_TELEMETRY: { _participants.SetTelemetryData((CarTelemetryPacket)packet); break; }
                case PacketType.CAR_STATUS: { _participants.SetCarStatusData((CarStatusPacket)packet); break; }
                case PacketType.FINAL_CLASSIFICATION: { break; };
                case PacketType.LOBBY_INFO: { break; };
                case PacketType.EVENT:
                    {
                        //Id what type of event packet this is
                        EventType eventType = EventPacket.EventPacketType(packet.Data);

                        switch (eventType)
                        {
                            //return base class EventPacket if nothing more needs to be known than the event occured!
                            case EventType.Session_Started: { _eventManager.InvokeSessionStartedEvent(packet); break; }
                            case EventType.Session_Ended: { _eventManager.InvokeSessionEndedEvent(packet); break; }
                            case EventType.DRS_Enabled: { _eventManager.InvokeDRSEnabledEvent(packet); break; }
                            case EventType.DRS_Disabled: { _eventManager.InvokeDRSDisabledEvent(packet); break; }
                            case EventType.Chequered_Flag: { _eventManager.InvokeChequeredFlagEvent(packet); break; }

                            case EventType.Fastest_Lap: { _eventManager.InvokeFastestLapEvent(packet); break; }
                            case EventType.Retirement: { UnityEngine.Debug.LogWarning("Retirement: No implemented reaction to this event!"); break; }
                            case EventType.Team_Mate_In_Pits: { UnityEngine.Debug.LogWarning("Team mate in pits: No implemented reaction to this event!"); break; }
                            case EventType.Race_Winner: { UnityEngine.Debug.LogWarning("Race Winner: No implemented reaction to this event!"); break; }
                            case EventType.Penalty_Issued: { _eventManager.InvokePenaltyEvent(packet); break; }
                            case EventType.Speed_Trap_Triggered: { UnityEngine.Debug.LogWarning("Speed trap triggered: No implemented reaction to this event!"); break; }
                            default: { throw new System.Exception("There is no handle support for this event: " + eventType); }
                        }
                        break;
                    }
                default: { throw new System.Exception("There is no handle support for this packet: " + (PacketType)packet.PacketID); }
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
                case (PacketType.MOTION): { return new MotionPacket(data); }
                case (PacketType.SESSION): { return new SessionPacket(data); }
                case (PacketType.LAP_DATA): { return new LapDataPacket(data); }
                case (PacketType.PARTICIPANTS): { return new ParticipantsPacket(data); }
                case (PacketType.CAR_SETUPS): { return new CarSetupPacket(data); }
                case (PacketType.CAR_TELEMETRY): { return new CarTelemetryPacket(data); }
                case (PacketType.CAR_STATUS): { return new CarStatusPacket(data); }
                case (PacketType.FINAL_CLASSIFICATION): { return new FinalClassificationPacket(data); }
                case (PacketType.LOBBY_INFO): { return new LobbyInfoPacket(data); }
                case (PacketType.EVENT):
                    {
                        //Id what type of event packet this is
                        EventType eventType = EventPacket.EventPacketType(data);

                        switch (eventType)
                        {
                            //return base class EventPacket if nothing more needs to be known than the event occured!
                            case EventType.Session_Started: { return new EventPacket(data); }
                            case EventType.Session_Ended: { return new EventPacket(data); }
                            case EventType.DRS_Enabled: { return new EventPacket(data); }
                            case EventType.DRS_Disabled: { return new EventPacket(data); }
                            case EventType.Chequered_Flag: { return new EventPacket(data); }

                            case EventType.Fastest_Lap: { return new FastestLapEventPacket(data); }
                            case EventType.Retirement: { return new RetirementEventPacket(data); }
                            case EventType.Team_Mate_In_Pits: { return new TeamMateInPitsEventPacket(data); }
                            case EventType.Race_Winner: { return new RaceWinnerEventPacket(data); }
                            case EventType.Penalty_Issued: { return new PenaltyEventPacket(data); }
                            case EventType.Speed_Trap_Triggered: { return new SpeedTrapEventPacket(data); }
                            default: { throw new System.Exception("There exist no such eventype: " + eventType); }
                        }
                    }
                default: { throw new System.Exception("There exist no such packet type: " + packetType); }
            }
        }
    }
}
