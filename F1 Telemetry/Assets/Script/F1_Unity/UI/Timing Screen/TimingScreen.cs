using System.Collections.Generic;
using UnityEngine;
using F1_Data_Management;
using System.Linq;

namespace F1_Unity
{
    public class TimingScreen : MonoBehaviour
    {
        #region Fields

        public static readonly float LEADER_LAP_EPSILON = 0.01f;

        [SerializeField, Range(10, 400)] int _amountOfTimingStations = 100;
        [SerializeField, Range(0.01f, 5f)] float _flashColorDuration = 1.0f;
        [SerializeField, Range(0.01f, 50f)] float _changeBackColorDuration = 4.5f;
        [SerializeField] Color _movedUpColor = Color.green;
        [SerializeField] Color _movedDownColor = Color.red;
        [SerializeField] CanvasGroup _canvasGroup;
        [SerializeField] DriverTemplate[] _driverTemplates;

        //Reach driver position by their ID
        Dictionary<byte, int> _driverPosition;
        Dictionary<byte, int> _driverLastTimeSpot;
        Dictionary<byte, DriverFinishData> _deltaToLeaderFinish = new Dictionary<byte, DriverFinishData>();

        //TEST
        List<DriverFinishData> _testDeltaToLeaderFinishData = new List<DriverFinishData>();
        //TEST

        TimingStation[] _timingData;
        int _leaderVehicleIndex;
        int _lastLeaderVehicleIndex = int.MinValue;
        int _lastPassedTimingIndex;
        bool _initValues = true;

        TimeScreenState _timeScreenState = TimeScreenState.Leader;
        TimingStats.TimingStatsState _timingStatsState = TimingStats.TimingStatsState.None;

        #endregion

        #region Start / End Methods

        private void Awake()
        {
            Init();
        }

        private void OnEnable()
        {
            GameManager.InputManager.PressedTimeInterval += ChangeTimingMode;
            GameManager.InputManager.PressedTimeStatsState += ChangeTimingState;
        }

        private void OnDisable()
        {
            GameManager.InputManager.PressedTimeInterval -= ChangeTimingMode;
            GameManager.InputManager.PressedTimeStatsState -= ChangeTimingState;
        }

        /// <summary>
        /// Shows or not show timing screen (will still be active in background)
        /// </summary>
        public void SetActive(bool status)
        {
            _canvasGroup.alpha = status ? 1.0f : 0.0f;
        }

        #endregion

        #region Init Functions

        /// <summary>
        /// Assign correct number to each placement and create singleton
        /// </summary>
        void Init()
        {
            _timingData = new TimingStation[_amountOfTimingStations];

            for (int i = 0; i < _driverTemplates.Length; i++)
                _driverTemplates[i].Init(i + 1, _flashColorDuration, _changeBackColorDuration);
        }


        /// <summary>
        /// Maps driver IDs to position on track. Used to compare old position to current
        /// </summary>
        void InitDrivers()
        {
            _initValues = false;

            SetMode(_timeScreenState);

            _driverPosition = new Dictionary<byte, int>();
            _driverLastTimeSpot = new Dictionary<byte, int>();

            Session sessionData = GameManager.F1Info.ReadSession(out bool status);

            for (int i = 0; i < F1Info.MAX_AMOUNT_OF_CARS; i++)
            {
                DriverData driverData = GameManager.F1Info.ReadCarData(i, out bool validDriver);

                ResultStatus resultStatus = ResultStatus.Inactive;

                //Init everyone with gaining a position on start!
                //Flash green then to white
                //Only add valid drivers to the grid
                if (validDriver && status)
                {
                    _driverPosition.Add(driverData.ID, driverData.LapData.carPosition + 1);
                    _driverTemplates[i].SetTimingState(DriverTimeState.Starting);
                    _driverTemplates[i].SetTiming();
                    //Sets driverdata if it's usage is needed
                    _driverTemplates[GetTemplateIndex(driverData)].SetDriverData(driverData);

                    //If the car is already retired -> set it so
                    resultStatus = driverData.LapData.resultStatus;
                    if (resultStatus == ResultStatus.Retired || resultStatus == ResultStatus.Disqualified)
                        _driverTemplates[i].Out(resultStatus);
                    else
                        _driverTemplates[i].NotOut();
                    //Init leader
                    if (driverData.LapData.carPosition == 1)
                    {
                        float lapCompletion = LapCompletion(sessionData, driverData);
                        int timingIndex = (int)(lapCompletion * (_amountOfTimingStations - 1));
                        _lastPassedTimingIndex = timingIndex;

                        _leaderVehicleIndex = driverData.VehicleIndex;
                    }
                }

                //Only enable so many positions in time standing as there are active drivers
                //If they DNF/DSQ later they will only gray out, not be removed
                _driverTemplates[i].SetActive(!(resultStatus == ResultStatus.Inactive || resultStatus == ResultStatus.Invalid));
            }

            //Need to loop again to set timing index same as leader -> Will make sure no odd times are displayed if restarting mid race!
            for (int i = 0; i < F1Info.MAX_AMOUNT_OF_CARS; i++)
            {
                DriverData driverData = GameManager.F1Info.ReadCarData(i, out bool validDriver);

                if (validDriver && status)
                    _driverLastTimeSpot.Add(driverData.ID, _lastPassedTimingIndex);
            }
        }

        #endregion

        #region Modes

        /// <summary>
        /// Changes betwen interval mode and to leader
        /// </summary>
        void ChangeTimingMode()
        {
            _timeScreenState = (TimeScreenState)(((int)_timeScreenState + 1) % (int)TimeScreenState.Length);
            SetMode(_timeScreenState);
        }

        /// <summary>
        /// Sets mode to interval or to leader
        /// </summary>
        void SetMode(TimeScreenState timeScreenState)
        {
            for (int i = 0; i < _driverTemplates.Length; i++)
                _driverTemplates[i].SetMode(timeScreenState);
        }

        /// <summary>
        /// Called when changing timing stats state -> changes all templates to that state
        /// </summary>
        void ChangeTimingState()
        {
            _timingStatsState = (TimingStats.TimingStatsState)(((int)_timingStatsState + 1) % (int)TimingStats.TimingStatsState.Length);
            SetStatsState(_timingStatsState);
        }

        /// <summary>
        /// Updates all templates to this stats state
        /// </summary>
        void SetStatsState(TimingStats.TimingStatsState state)
        {
            for (int i = 0; i < _driverTemplates.Length; i++)
                _driverTemplates[i].SetStatsState(state);
        }

        #endregion

        #region Update Methods

        //Updates standing
        private void Update()
        {
            //ONLY IN RACE!

            //Only update standings when data can be read safely and correctly
            if (GameManager.F1Info.ReadyToReadFrom)
            {
                if (_initValues)
                    InitDrivers();
                else
                    DoTimingScreen();
            }
        }

        /// <summary>
        /// Updates positions in standing and checks stability, also updates driver stats
        /// </summary>
        void DoTimingScreen()
        {
            //Will be valid -> checked earlier
            Session sessionData = GameManager.F1Info.ReadSession(out bool status);
            DriverData leaderData = GameManager.F1Info.ReadCarData(_leaderVehicleIndex, out bool status2);
            UpdateTimeStationByLeader(sessionData, leaderData);

            //Loop through all drivers
            for (int i = 0; i < F1Info.MAX_AMOUNT_OF_CARS; i++)
            {
                DriverData driverData = GameManager.F1Info.ReadCarData(i, out bool validDriver);
                //Skip the drivers that have no valid index -> junk data
                if (!validDriver)
                    continue;
                DoDriver(driverData, leaderData, sessionData);
            }

            //Set time text for each template and calculate interval based on DeltaToLeader
            _driverTemplates[0].SetTiming(); //Leader
            for (int i = 1; i < _driverTemplates.Length; i++)
            {
                float previousCarDeltaToLeader = _driverTemplates[i - 1].DeltaToLeader;
                _driverTemplates[i].SetCarAheadDelta(previousCarDeltaToLeader);
                _driverTemplates[i].SetTiming();
            }
        }

        /// <summary>
        /// Sets all aspect of timing for specific driver
        /// </summary>
        void DoDriver(DriverData driverData, DriverData leaderData, Session sessionData)
        {
            int index = GetTemplateIndex(driverData);
            //Set specific driver info to template
            _driverTemplates[index].SetDriverData(driverData);
            _driverTemplates[index].SetFastestLap(driverData);
            //Update stats for driver
            _driverTemplates[index].UpdateStats(driverData, _timingStatsState);

            Positioning(driverData, sessionData, index);
            //Need to read leaderData again in case this driver overtook him
            leaderData = GameManager.F1Info.ReadCarData(_leaderVehicleIndex, out bool status3);

            //Driver has finished
            if (driverData.LapData.resultStatus == ResultStatus.Finished)
                DriverFinish(driverData, leaderData, index);
            //Driver is racing
            else if (driverData.LapData.currentLapNumber <= sessionData.TotalLaps)
            {
                bool inPit = IsDriverInPit(driverData, index);
                UpdateDriverTimingToLeader(leaderData, sessionData, driverData, inPit);
            }
        }

        #endregion

        #region Timestations

        /// <summary>
        /// Saves time and lap leader passed a timing station for other cars to be able to compare with.
        /// </summary>
        void UpdateTimeStationByLeader(Session sessionData, DriverData leaderData)
        {
            //Find closest time station
            float lapCompletion = LapCompletion(sessionData, leaderData);
            int timingIndex = (int)(lapCompletion * (_amountOfTimingStations - 1));
            //It's a new time station + don't update already updated station
            if (timingIndex != _lastPassedTimingIndex)
            {
                //Leader has passed more than one time station since last
                //-> Copy previous timestation data to inbetween stations
                if (Modulo((timingIndex - _lastPassedTimingIndex), _amountOfTimingStations) > 1)
                    SetMissedTimeStations(timingIndex);

                _timingData[timingIndex].Time = GameManager.F1Info.SessionTime;
                _timingData[timingIndex].Lap = leaderData.LapData.currentLapNumber;
                _timingData[timingIndex].PassedByLeader = true;
                _timingData[timingIndex].VehicleIndex = leaderData.VehicleIndex;
                _lastPassedTimingIndex = timingIndex;
            }
        }

        /// <summary>
        /// The leader has missed a(many) timestations, fill them in with values based on current timestation
        /// </summary>
        void SetMissedTimeStations(int timingIndex)
        {
            int timingStationIndex = Modulo(_lastPassedTimingIndex + 1, _amountOfTimingStations);
            while (timingStationIndex != timingIndex)
            {
                _timingData[timingStationIndex].Time = _timingData[_lastPassedTimingIndex].Time;
                //If between station is ahead it's on the same lap -> otherwise the next lap
                if (timingStationIndex > _lastPassedTimingIndex)
                    _timingData[timingStationIndex].Lap = _timingData[_lastPassedTimingIndex].Lap;
                else
                    _timingData[timingStationIndex].Lap = (byte)(_timingData[_lastPassedTimingIndex].Lap + 1);
                _timingData[timingStationIndex].PassedByLeader = true;
                _timingData[timingStationIndex].VehicleIndex = _timingData[_lastPassedTimingIndex].VehicleIndex;
                timingStationIndex = Modulo(timingStationIndex + 1, _amountOfTimingStations);
            }
        }

        #endregion

        #region Finish

        /// <summary>
        /// A driver just crossed the finish line, calculate interval to leader with penalties
        /// </summary>
        /// <param name="index">Index to access driver template</param>
        void DriverFinish(DriverData driverData, DriverData leaderData, int index)
        {
            //Just crossed line!
            if (!_deltaToLeaderFinish.ContainsKey(driverData.ID))
            {
                //Leader just crossed the line (could be new leader after penalties)
                if (leaderData.ID == driverData.ID)
                {
                    //First time skip this -> it switches leader target for finish
                    if (_lastLeaderVehicleIndex != int.MinValue)
                    {
                        DriverData oldLeaderData = GameManager.F1Info.ReadCarData(_lastLeaderVehicleIndex, out bool status);
                        _driverTemplates[GetTemplateIndex(oldLeaderData)].SetTimingState(DriverTimeState.Delta);
                        _driverTemplates[GetTemplateIndex(driverData)].SetTimingState(DriverTimeState.Leader);

                        DriverFinishData oldLeaderFinishData = _deltaToLeaderFinish[oldLeaderData.ID];
                        float deltaToLeader =  oldLeaderFinishData.TimeStamp - GameManager.F1Info.SessionTime;
                        oldLeaderFinishData.TimeToLeader = deltaToLeader;
                        _deltaToLeaderFinish[oldLeaderData.ID] = oldLeaderFinishData;
                        _driverTemplates[GetTemplateIndex(oldLeaderData)].SetDeltaToLeader(oldLeaderFinishData.IntervalToLeader);

                        //TESTING
                        _testDeltaToLeaderFinishData.Add(oldLeaderFinishData);
                        Debug.Log("Changed leader!");
                        Debug.Log("Delta to new leader: " + oldLeaderFinishData.IntervalToLeader);
                        //TESTING
                    }
                    //The new leaders finish data
                    _deltaToLeaderFinish.Add(driverData.ID, new DriverFinishData()
                    {
                        TimeToLeader = 0,
                        TimeStamp = GameManager.F1Info.SessionTime,
                        Penalties = driverData.LapData.totalPenalties,
                        VehicleIndex = driverData.VehicleIndex
                    });

                    //TESTING
                    _testDeltaToLeaderFinishData.Add(_deltaToLeaderFinish[driverData.ID]);
                    //TESTING

                    _lastLeaderVehicleIndex = driverData.VehicleIndex;
                    UpdateAllFinishIntervalAfterLeader(_deltaToLeaderFinish[driverData.ID]); //If this is a new leader it updates delta after this
                }
                //Driver finish behind leader
                else
                {
                    DriverFinishData leaderFinishData = _deltaToLeaderFinish[leaderData.ID];
                    float deltaToLeader = GameManager.F1Info.SessionTime - leaderFinishData.TimeStamp - leaderFinishData.Penalties;
                    _deltaToLeaderFinish.Add(driverData.ID, new DriverFinishData()
                    {
                        TimeToLeader = deltaToLeader,
                        TimeStamp = GameManager.F1Info.SessionTime,
                        Penalties = driverData.LapData.totalPenalties,
                        VehicleIndex = driverData.VehicleIndex
                    });
                    //TEST
                    _testDeltaToLeaderFinishData.Add(_deltaToLeaderFinish[driverData.ID]);
                    //TEST
                }

                //Set the correct interval to leader
                _driverTemplates[index].SetDeltaToLeader(_deltaToLeaderFinish[driverData.ID].IntervalToLeader);
            }
        }

        /// <summary>
        /// If leader changes after penalty the intervals for all finished drivers must be updated from that reference instead of previous
        /// </summary>
        void UpdateAllFinishIntervalAfterLeader(DriverFinishData leaderData)
        {
            List<DriverFinishData> finishedDrivers = _deltaToLeaderFinish.Values.ToList();

            for (int i = 0; i < finishedDrivers.Count; i++)
            {
                //Update values to new reference point
                if (finishedDrivers[i].VehicleIndex != _lastLeaderVehicleIndex)
                {
                    DriverFinishData data = finishedDrivers[i];
                    float deltaToLeader = data.TimeStamp - leaderData.TimeStamp;
                    data.TimeToLeader = deltaToLeader;
                    DriverData driverData = GameManager.F1Info.ReadCarData(data.VehicleIndex, out bool status);
                    _deltaToLeaderFinish[driverData.ID] = data;
                    _driverTemplates[GetTemplateIndex(driverData)].SetDeltaToLeader(data.IntervalToLeader);
                    //TEST
                    Debug.Log("Fix interval: " + i + ": " + GameManager.ParticipantManager.GetNameFromNumber(driverData.RaceNumber));
                    //TEST
                }
            }
        }

        #endregion

        #region Positioning

        /// <summary>
        /// Takes care of positioning and coloring of overtakes
        /// </summary>
        void Positioning(DriverData driverData, Session sessionData, int index)
        {
            byte carPosition = driverData.LapData.carPosition;

            //If this driver doesn't exist, it has just joined the session, recalculate everything!
            if (!_driverPosition.ContainsKey(driverData.ID) || !_driverLastTimeSpot.ContainsKey(driverData.ID))
                InitDrivers();

            //Drivers position has changed! Update!
            if (_driverPosition[driverData.ID] != carPosition)
                ChangeDriverPosition(carPosition, driverData, sessionData, index);
        }

        /// <summary>
        /// Change driver position and exchange values between templates -> update leader values if leader changed
        /// </summary>
        void ChangeDriverPosition(byte carPosition, DriverData driverData, Session sessionData, int index)
        {
            //Update indexes for leader
            if (carPosition == 1)
                ChangeLeader(driverData, sessionData);

            _driverTemplates[index].SetInitials(GameManager.ParticipantManager.GetDriverInitials(driverData.RaceNumber)); //Set initals for that position
            _driverTemplates[index].SetTeamColor(GameManager.F1Utility.GetColorByTeam(driverData.ParticipantData.team)); //Set team color

            //Set delta to leader if finished
            if (_deltaToLeaderFinish.ContainsKey(driverData.ID))
                _driverTemplates[index].SetDeltaToLeader(_deltaToLeaderFinish[driverData.ID].IntervalToLeader);

            //if the car is retired, set it to out
            ResultStatus resultStatus = driverData.LapData.resultStatus;
            if (resultStatus == ResultStatus.Retired || resultStatus == ResultStatus.Disqualified)
                _driverTemplates[index].Out(resultStatus);
            else
            {
                //If it was previously out make it not
                if (_driverTemplates[index].OutOfSession)
                    _driverTemplates[index].NotOut();

                //Change color wether driver GAINED or LOST to this position -> compare old position with this one
                _driverTemplates[index].UpdatePositionColor(_driverPosition[driverData.ID], _movedUpColor, _movedDownColor);
            }

            //save this position to compare in future
            _driverPosition[driverData.ID] = carPosition;
        }

        /// <summary>
        /// An overtake has happened which changed the leader -> update indexes accordingly
        /// </summary>
        void ChangeLeader(DriverData driverData, Session sessionData)
        {
            //Set the timing index to be from where car is currently 
            //(if normal overtake it will be the same as last, if overtake is on penalties at finish it will move without changing others)
            float lapCompletion = LapCompletion(sessionData, driverData);
            int timingIndex = (int)(lapCompletion * (_amountOfTimingStations - 1));
            _lastPassedTimingIndex = timingIndex;

            _leaderVehicleIndex = driverData.ParticipantData.vehicleIndex;
        }

        #endregion

        #region Driver Update

        /// <summary>
        /// Checks if driver is currently in pit and sets it so 
        /// </summary>
        bool IsDriverInPit(DriverData driverData, int index)
        {
            if (driverData.LapData.pitStatus == PitStatus.Pitting)
                _driverTemplates[index].SetTimingState(DriverTimeState.Pit);
            else if (driverData.LapData.pitStatus == PitStatus.In_Pit_Area)
                _driverTemplates[index].SetTimingState(DriverTimeState.Pit_Area);
            else
                return false;
            return true;
        }

        /// <summary>
        /// Sets the time text for a driver depending on distance to leader.
        /// </summary>
        void UpdateDriverTimingToLeader(DriverData leaderData, Session sessionData, DriverData driverData, bool inPit)
        {
            int index = GetTemplateIndex(driverData);

            float lapCompletion = LapCompletion(sessionData, driverData);
            float leaderLapCompletion = LapCompletion(sessionData, leaderData);
            int timingIndex = (int)(lapCompletion * (_amountOfTimingStations - 1));

            //No need to update since it hasn't passed a new station
            if (_driverLastTimeSpot[driverData.ID] == timingIndex)
                return;
            //Update last time spot for this driver
            else
                _driverLastTimeSpot[driverData.ID] = timingIndex;

            //Index of current timing to compare with
            float currentTime = GameManager.F1Info.SessionTime;
            float deltaToLeader = currentTime - _timingData[timingIndex].Time;

            _driverTemplates[index].SetDeltaToLeader(deltaToLeader);
            bool passedLeader = _timingData[timingIndex].PassedByLeader;

            //This is the leader!
            if (leaderData.VehicleIndex == driverData.VehicleIndex)
            {
                //It is leader so delta to itself is 0
                _driverTemplates[index].SetDeltaToLeader(0);
                if (!inPit)
                    _driverTemplates[index].SetTimingState(DriverTimeState.Leader);
            }
            else if (IsLapped(leaderLapCompletion, lapCompletion, leaderData, driverData, sessionData, out int amountOfLaps))
            {
                if (!inPit)
                    _driverTemplates[index].SetTimingState(DriverTimeState.Lapped);
                _driverTemplates[index].SetLapsLapped(amountOfLaps);
            }
            else if (!passedLeader && !inPit)
                _driverTemplates[index].SetTimingState(DriverTimeState.Starting);
            //If nothing else -> Show delta
            else if (!inPit)
                _driverTemplates[index].SetTimingState(DriverTimeState.Delta);

        } 

        /// <summary>
        /// Returns weather a car is lapped by a leader
        /// </summary>
        /// <param name="leaderLapCompletion">How far along the current lap the leader is (0-1)</param>
        /// <param name="lapCompletion">How far along the current lap the driver is (0-1)</param>
        /// <param name="leaderData">DriverData for leader</param>
        /// <param name="driverData">DriverData for driver</param>
        /// <param name="amountOfLaps">out parameter for how many laps the driver is lapped if any</param>
        /// <returns>true if driver is lapped by leader</returns>
        bool IsLapped(float leaderLapCompletion, float lapCompletion, DriverData leaderData, DriverData driverData, Session sessionData, out int amountOfLaps)
        {
            amountOfLaps = leaderData.LapData.currentLapNumber - driverData.LapData.currentLapNumber;

            //Sometimes values around 0.98 < value < 1.0 might still be on a new lap, add epsilon and overflow  
            leaderLapCompletion += LEADER_LAP_EPSILON;
            leaderLapCompletion = leaderLapCompletion > 1 ? leaderLapCompletion - 1 : leaderLapCompletion;

            //Could be that this car hasn't crossed finish line yet!
            if (lapCompletion > leaderLapCompletion)
                amountOfLaps--;
            return amountOfLaps > 0;
        }

        #endregion

        #region Help Methods

        /// <summary>
        /// How far along a lap is a driver? (0.0f - 1.0f)
        /// </summary>
        float LapCompletion(Session sessionData, DriverData driverData)
        {
            return Mathf.Clamp01(driverData.LapData.lapDistance / sessionData.TrackLength);
        }

        /// <summary>
        /// Returns current lap for a driver clamped to amount of lap in session
        /// </summary>
        /// <returns></returns>
        int GetCurrentLapClamped(DriverData driverData, Session sessionData)
        {
            return Mathf.Clamp(driverData.LapData.currentLapNumber, 0, sessionData.TotalLaps);
        }

        /// <summary>
        /// Return the index for template based on driver position
        /// </summary>
        int GetTemplateIndex(DriverData driverData)
        {
            return driverData.LapData.carPosition - 1;
        }    

        /// <summary>
        /// Better modulo than % with only giving positive values
        /// </summary>
        int Modulo(int x, int m)
        {
            int r = x % m;
            return r < 0 ? r + m : r;
        }

        #endregion

        #region Structs

        /// <summary>
        /// A timing station that holds time and lap when leader passed
        /// </summary>
        [System.Serializable]
        struct TimingStation
        {
            public bool PassedByLeader { get; set; }
            public byte Lap { get; set; }
            public float Time { get; set; }
            public int VehicleIndex { get; set; }
        }

        /// <summary>
        /// Used to set the correct timing when finishing the race
        /// </summary>
        [System.Serializable]
        struct DriverFinishData
        {
            /// <summary>
            /// The total time interval to the leader with penalties
            /// </summary>
            public float IntervalToLeader { get { return TimeToLeader + Penalties;  } }
            /// <summary>
            /// Total penalties this driver has in seconds
            /// </summary>
            public float Penalties { get; set; }
            /// <summary>
            /// The time to leader WITHOUT penalties when crossing finish line to end race
            /// </summary>
            public float TimeToLeader { get; set; }
            /// <summary>
            /// The time in game the driver crossed the finish line
            /// </summary>
            public float TimeStamp { get; set; }
            /// <summary>
            /// The vehicle index this data applies to
            /// </summary>
            public int VehicleIndex { get; set; }
        }

        #endregion
    }

    #region Enums

    /// <summary>
    /// Used to tell what state timing screen is in
    /// </summary>
    public enum TimeScreenState
    {
        None,
        Leader,
        Interval,
        Fastest_Lap,
        Length
    }

    #endregion
}
