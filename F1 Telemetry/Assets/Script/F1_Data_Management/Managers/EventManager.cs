namespace F1_Data_Management
{
    public delegate void EventOccour(Packet packet);

    /// <summary>
    /// Holds events that trigger when certain events occour. Subscribe to them to react to those events.
    /// </summary>
    public class EventManager
    {
        #region Events
        //Complex Events
        /// <summary>
        /// Invoked on fastest lap event. Returns Packet which can be cast to FastestLapEventPacket.
        /// </summary>
        public event EventOccour FastestLapEvent;
        /// <summary>
        /// Invoked on penalty event. Returns Packet which can be cast to PenaltyEventPacket.
        /// </summary>
        public event EventOccour PenaltyEvent;
        /// <summary>
        /// Invoked on Race Winner event. Returns Packet which can be cast to RaceWinnerEventPacket.
        /// </summary>
        public event EventOccour RaceWinnerEvent;
        /// <summary>
        /// Invoked on Retirement event. Returns Packet which can be cast to RetirementEventPacket.
        /// </summary>
        public event EventOccour RetirementEvent;
        /// <summary>
        /// Invoked on SpeedTrap event. Returns Packet which can be cast to SpeedTrapEventPacket.
        /// </summary>
        public event EventOccour SpeedTrapEvent;
        /// <summary>
        /// Invoked on Teammate in pits event. Returns Packet which can be cast to TeamMateInPitsEventPacket.
        /// </summary>
        public event EventOccour TeamMateInPitsEvent;
        //Easy Events
        /// <summary>
        /// Invoked on DRS enabled event. Returns Packet which can be cast to EventPacket.
        /// </summary>
        public event EventOccour DRSEnabledEvent;
        /// <summary>
        /// Invoked on DRS disabled event. Returns Packet which can be cast to EventPacket.
        /// </summary>
        public event EventOccour DRSDisabledEvent;
        /// <summary>
        /// Invoked on Chequered flag event. When car is about to finish the race. Returns Packet which can be cast to EventPacket.
        /// </summary>
        public event EventOccour ChequeredFlagEvent;
        /// <summary>
        /// Invoked on Session started event. Returns Packet which can be cast to EventPacket.
        /// </summary>
        public event EventOccour SessionStartedEvent;
        /// <summary>
        /// Invoked on Session ended event. Returns Packet which can be cast to EventPacket.
        /// </summary>
        public event EventOccour SessionEndedEvent;
        #endregion

        #region Invokers
        /// <summary>
        /// Called from PacketManager when fastest lap event occour.
        /// </summary>
        public void InvokeFastestLapEvent(Packet packet)
        {
            FastestLapEvent?.Invoke(packet);
        }

        /// <summary>
        /// Called from PacketManager when penalty event occour.
        /// </summary>
        public void InvokePenaltyEvent(Packet packet)
        {
            PenaltyEvent?.Invoke(packet);
        }

        /// <summary>
        /// Called from PacketManager when Race Winner event occour. Event not triggered in spectator mode.
        /// </summary>
        public void InvokeRaceWinnerEvent(Packet packet)
        {
            RaceWinnerEvent?.Invoke(packet);
        }

        /// <summary>
        /// Called from PacketManager when Retirement event occour.
        /// </summary>
        public void InvokeRetirementEvent(Packet packet)
        {
            RetirementEvent?.Invoke(packet);
        }

        /// <summary>
        /// Called from PacketManager when Speed Trap event occour.
        /// </summary>
        public void InvokeSpeedTrapEvent(Packet packet)
        {
            SpeedTrapEvent?.Invoke(packet);
        }

        /// <summary>
        /// Called from PacketManager when Team Mate In Pits event occour. Event not triggered in spectator mode.
        /// </summary>
        public void InvokeTeamMateInPitsEvent(Packet packet)
        {
            TeamMateInPitsEvent?.Invoke(packet);
        }

        /// <summary>
        /// Called from PacketManager when DRS enabled event occour.
        /// </summary>
        public void InvokeDRSEnabledEvent(Packet packet)
        {
            DRSEnabledEvent?.Invoke(packet);
        }

        /// <summary>
        /// Called from PacketManager when DRS disabled event occour.
        /// </summary>
        public void InvokeDRSDisabledEvent(Packet packet)
        {
            DRSDisabledEvent?.Invoke(packet);
        }

        /// <summary>
        /// Called from PacketManager when car is about to cross the finish line for last lap. Event not triggered in spectator mode.
        /// </summary>
        public void InvokeChequeredFlagEvent(Packet packet)
        {
            ChequeredFlagEvent?.Invoke(packet);
        }

        /// <summary>
        /// Called from PacketManager when session starts.
        /// </summary>
        public void InvokeSessionStartedEvent(Packet packet)
        {
            SessionStartedEvent?.Invoke(packet);
        }

        /// <summary>
        /// Called from PacketManager when session ends.
        /// </summary>
        public void InvokeSessionEndedEvent(Packet packet)
        {
            SessionEndedEvent?.Invoke(packet);
        }

        #endregion
    }
}