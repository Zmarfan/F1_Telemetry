using System;

namespace F1_Data_Management
{
    /// <summary>
    /// User entry to use the functions of F1_Data_Management library.
    /// </summary>
    public class F1Info
    {
        public const int MAX_AMOUNT_OF_CARS = 22;

        PacketManager _packetManager;
        UdpReceiver _udpReceiver;
        Participants _participants;
        EventManager _eventManager;

        /// <summary>
        /// Amount of drivers actually competing -> Indexes can fall outside this value! Don't use for indexing! 0 if not in use
        /// </summary>
        public int ActiveDrivers { get { return _participants.ActiveDrivers; } }
        /// <summary>
        /// Only read data when it's actually there
        /// </summary>
        public bool ReadyToReadFrom { get { return _participants.ReadyToReadFrom; } }

        public F1Info(int port = 20777)
        {
            _eventManager = new EventManager();
            _participants = new Participants();
            _packetManager = new PacketManager(_participants, _eventManager);
            _udpReceiver = new UdpReceiver(_packetManager, port);
        }

        /// <summary>
        /// Start listening and collecting incoming data packets from F1 2020.
        /// </summary>
        public void Start()
        {
            _udpReceiver.StartListening();
        }

        /// <summary>
        /// Will read the packets into data that has been collected since last called.
        /// </summary>
        public void ReadCollectedPackets()
        {
            _packetManager.ReadCollectedPackets();
        }

        /// <summary>
        /// Empties all collected data and stop listening for new incoming data.
        /// </summary>
        public void Reset()
        {
            _udpReceiver.StopListening();
            _packetManager.Reset();
            _participants.Clear();
        }

        /// <summary>
        /// Attempt to read data for vehicle. validData indicates if data returned is valid data.
        /// </summary>
        /// <param name="vehicleIndex">Index of the car which data you want to get. Must be within 0 - 22.</param>
        /// <param name="validData">Indicates if returned data is valid data. Unvalid means either -> vehicle doesn't exist or data not yet set</param>
        public DriverData ReadCarData(int vehicleIndex, out bool validData)
        {
            return _participants.ReadCarData(vehicleIndex, out validData);
        }
        #region Events

        /// <summary>
        /// Invoked on fastest lap event. Returns Packet which can be cast to FastestLapEventPacket.
        /// </summary>
        public event EventOccour FastestLapEvent
        {
            add => _eventManager.FastestLapEvent += value;
            remove => _eventManager.FastestLapEvent -= value;
        }

        /// <summary>
        /// Invoked on penalty event. Returns Packet which can be cast to PenaltyEventPacket.
        /// </summary>
        public event EventOccour PenaltyEvent
        {
            add => _eventManager.PenaltyEvent += value;
            remove => _eventManager.PenaltyEvent -= value;
        }

        /// <summary>
        /// Invoked on Race Winner event. Returns Packet which can be cast to RaceWinnerEventPacket. Event not triggered in spectator mode.
        /// </summary>
        public event EventOccour RaceWinnerEvent
        {
            add => _eventManager.RaceWinnerEvent += value;
            remove => _eventManager.RaceWinnerEvent -= value;
        }

        /// <summary>
        /// Invoked on Retirement event. Returns Packet which can be cast to RetirementEventPacket.
        /// </summary>
        public event EventOccour RetirementEvent
        {
            add => _eventManager.RetirementEvent += value;
            remove => _eventManager.RetirementEvent -= value;
        }

        /// <summary>
        /// Invoked on SpeedTrap event. Returns Packet which can be cast to SpeedTrapEventPacket.
        /// </summary>
        public event EventOccour SpeedTrapEvent
        {
            add => _eventManager.SpeedTrapEvent += value;
            remove => _eventManager.SpeedTrapEvent -= value;
        }

        /// <summary>
        /// Invoked on Teammate in pits event. Returns Packet which can be cast to TeamMateInPitsEventPacket. Event not triggered in spectator mode.
        /// </summary>
        public event EventOccour TeamMateInPitsEvent
        {
            add => _eventManager.TeamMateInPitsEvent += value;
            remove => _eventManager.TeamMateInPitsEvent -= value;
        }

        /// <summary>
        /// Invoked on DRS enabled event. Returns Packet which can be cast to EventPacket.
        /// </summary>
        public event EventOccour DRSEnabledEvent
        {
            add => _eventManager.DRSEnabledEvent += value;
            remove => _eventManager.DRSEnabledEvent -= value;
        }

        /// <summary>
        /// Invoked on DRS disabled event. Returns Packet which can be cast to EventPacket.
        /// </summary>
        public event EventOccour DRSDisabledEvent
        {
            add => _eventManager.DRSDisabledEvent += value;
            remove => _eventManager.DRSDisabledEvent -= value;
        }

        /// <summary>
        /// Invoked on Chequered flag event. When car is about to finish the race. Returns Packet which can be cast to EventPacket. Event not triggered in spectator mode.
        /// </summary>
        public event EventOccour ChequeredFlagEvent
        {
            add => _eventManager.ChequeredFlagEvent += value;
            remove => _eventManager.ChequeredFlagEvent -= value;
        }

        /// <summary>
        /// Invoked on Session started event. Returns Packet which can be cast to EventPacket.
        /// </summary>
        public event EventOccour SessionStartedEvent
        {
            add => _eventManager.SessionStartedEvent += value;
            remove => _eventManager.SessionStartedEvent -= value;
        }

        /// <summary>
        /// Invoked on Session ended event. Returns Packet which can be cast to EventPacket.
        /// </summary>
        public event EventOccour SessionEndedEvent
        {
            add => _eventManager.SessionEndedEvent += value;
            remove => _eventManager.SessionEndedEvent -= value;
        }
        #endregion
    }
}

