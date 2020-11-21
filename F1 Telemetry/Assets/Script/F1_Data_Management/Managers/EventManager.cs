namespace F1_Data_Management
{
    public delegate void EventOccour(Packet packet);

    /// <summary>
    /// Holds events that trigger when certain events occour. Subscribe to them to react to those events.
    /// </summary>
    public class EventManager
    {
        #region Events
        /// <summary>
        /// Invoked on fastest lap event. Returns Packet which can be cast to FastestLapEventPacket.
        /// </summary>
        public static event EventOccour FastestLapEvent;
        /// <summary>
        /// Invoked on penalty event. Returns Packet which can be cast to PenaltyEventPacket.
        /// </summary>
        public static event EventOccour PenaltyEvent;
        /// <summary>
        /// Invoked on Race Winner event. Returns Packet which can be cast to RaceWinnerEventPacket.
        /// </summary>
        public static event EventOccour RaceWinnerEvent;
        /// <summary>
        /// Invoked on Retirement event. Returns Packet which can be cast to RetirementEventPacket.
        /// </summary>
        public static event EventOccour RetirementEvent;
        /// <summary>
        /// Invoked on SpeedTrap event. Returns Packet which can be cast to SpeedTrapEventPacket.
        /// </summary>
        public static event EventOccour SpeedTrapEvent;
        /// <summary>
        /// Invoked on Teammate in pits event. Returns Packet which can be cast to TeamMateInPitsEventPacket.
        /// </summary>
        public static event EventOccour TeamMateInPitsEvent;
        #endregion

        #region Invokers
        /// <summary>
        /// Called from PacketManager when fastest lap event occour.
        /// </summary>
        public static void InvokeFastestLapEvent(Packet packet)
        {
            FastestLapEvent?.Invoke(packet);
        }

        /// <summary>
        /// Called from PacketManager when penalty event occour.
        /// </summary>
        public static void InvokePenaltyEvent(Packet packet)
        {
            PenaltyEvent?.Invoke(packet);
        }

        /// <summary>
        /// Called from PacketManager when Race Winner event occour.
        /// </summary>
        public static void InvokeRaceWinnerEvent(Packet packet)
        {
            RaceWinnerEvent?.Invoke(packet);
        }

        /// <summary>
        /// Called from PacketManager when Retirement event occour.
        /// </summary>
        public static void InvokeRetirementEvent(Packet packet)
        {
            RetirementEvent?.Invoke(packet);
        }

        /// <summary>
        /// Called from PacketManager when Speed Trap event occour.
        /// </summary>
        public static void InvokeSpeedTrapEvent(Packet packet)
        {
            SpeedTrapEvent?.Invoke(packet);
        }

        /// <summary>
        /// Called from PacketManager when Team Mate In Pits event occour.
        /// </summary>
        public static void InvokeTeamMateInPitsEvent(Packet packet)
        {
            TeamMateInPitsEvent?.Invoke(packet);
        }

        #endregion
    }
}