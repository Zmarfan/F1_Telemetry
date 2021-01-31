using System.Collections.Generic;
using UnityEngine;
using F1_Data_Management;
using System.Linq;

namespace F1_Unity
{
    public class RaceTimingScreen : TimingScreenBase
    {
        #region Fields

        static readonly float LEADER_LAP_EPSILON = 0.01f;

        [SerializeField, Range(10, 400)] int _amountOfTimingStations = 100;

        Dictionary<byte, int> _driverLastTimeSpot;
        Dictionary<byte, DriverFinishData> _deltaToLeaderFinish = new Dictionary<byte, DriverFinishData>();

        TimingStation[] _timingData;
        int _lastLeaderVehicleIndex = int.MinValue;
        int _lastPassedTimingIndex;

        #endregion

        #region public methods

        /// <summary>
        /// Completely resets to original prefab state -> used when changing between sessions
        /// </summary>
        public override void CompleteReset()
        { 
            _driverLastTimeSpot = null;
            _deltaToLeaderFinish = new Dictionary<byte, DriverFinishData>();

            _timingData = null;
            _lastLeaderVehicleIndex = int.MinValue;

            base.CompleteReset();
        }

        #endregion

        #region Init Functions

        /// <summary>
        /// Assign correct number to each placement and create singleton
        /// </summary>
        override protected void Init()
        {
            _timingData = new TimingStation[_amountOfTimingStations];

            base.Init();
        }


        /// <summary>
        /// Maps driver IDs to position on track. Used to compare old position to current
        /// </summary>
        override protected void InitDrivers()
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
                    _driverEntries[i].SetTimingState(DriverTimeState.Starting);
                    _driverEntries[i].SetTiming();
                    //Sets driverdata if it's usage is needed
                    _driverEntries[GetEntryIndex(driverData)].SetDriverData(driverData);

                    //If the car is already retired -> set it so
                    resultStatus = driverData.LapData.resultStatus;
                    if (resultStatus == ResultStatus.Retired || resultStatus == ResultStatus.Disqualified)
                        _driverEntries[i].Out(resultStatus);
                    else
                        _driverEntries[i].NotOut();
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
                _driverEntries[i].SetActive(!(resultStatus == ResultStatus.Inactive || resultStatus == ResultStatus.Invalid));
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

        #region Update Methods

        /// <summary>
        /// Updates positions in standing and checks stability, also updates driver stats
        /// </summary>
        override protected void MainUpdate()
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

            CalculateDriverIntervals();
        }

        /// <summary>
        /// Sets all aspect of timing for specific driver
        /// </summary>
        override protected void DoDriver(DriverData driverData, DriverData leaderData, Session sessionData)
        {
            int index = GetEntryIndex(driverData);
            //Set specific driver info to entry
            _driverEntries[index].SetDriverData(driverData);
            _driverEntries[index].SetFastestLap(driverData);
            _driverEntries[index].SetSpectator(driverData);

            //Update stats for driver
            _driverEntries[index].UpdateStats(driverData, _availableStatsState[_timingStatsStateIndex]);

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
                _driverEntries[index].SetInPit(inPit);
                UpdateDriverTimingToLeader(leaderData, sessionData, driverData, inPit, index);
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
                    bool firstLeader = _lastLeaderVehicleIndex != int.MinValue;

                    //First time skip this -> it switches leader target for finish
                    if (firstLeader)
                    {
                        DriverData oldLeaderData = GameManager.F1Info.ReadCarData(_lastLeaderVehicleIndex, out bool status);
                        _driverEntries[GetEntryIndex(oldLeaderData)].SetTimingState(DriverTimeState.Delta);
                        _driverEntries[GetEntryIndex(driverData)].SetTimingState(DriverTimeState.Leader);

                        DriverFinishData oldLeaderFinishData = _deltaToLeaderFinish[oldLeaderData.ID];
                        float deltaToLeader =  oldLeaderFinishData.TimeStamp - GameManager.F1Info.SessionTime;
                        oldLeaderFinishData.TimeToLeader = deltaToLeader;
                        _deltaToLeaderFinish[oldLeaderData.ID] = oldLeaderFinishData;
                        _driverEntries[GetEntryIndex(oldLeaderData)].SetDeltaToLeader(oldLeaderFinishData.IntervalToLeader);
                    }
                    //The new leaders finish data
                    _deltaToLeaderFinish.Add(driverData.ID, new DriverFinishData()
                    {
                        TimeToLeader = 0,
                        TimeStamp = GameManager.F1Info.SessionTime,
                        Penalties = driverData.LapData.totalPenalties,
                        VehicleIndex = driverData.VehicleIndex
                    });

                    _lastLeaderVehicleIndex = driverData.VehicleIndex;
                    UpdateAllFinishIntervalAfterLeader(_deltaToLeaderFinish[driverData.ID]); //If this is a new leader it updates delta after this

                    //Set the correct interval to leader
                    //First guy to cross line has 0 delta to itself
                    if (firstLeader)
                        _driverEntries[index].SetDeltaToLeader(0);
                    //If someone overtake leader on penalties, delta to leader will not be 0
                    else
                        _driverEntries[index].SetDeltaToLeader(_deltaToLeaderFinish[driverData.ID].IntervalToLeader);
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

                    //Set the correct interval to leader
                    _driverEntries[index].SetDeltaToLeader(_deltaToLeaderFinish[driverData.ID].IntervalToLeader);
                }
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

                    _driverEntries[GetEntryIndex(driverData)].SetDeltaToLeader(data.IntervalToLeader);
                }
            }
        }

        #endregion

        #region Positioning

        /// <summary>
        /// Change driver position and exchange values between entries -> update leader values if leader changed
        /// </summary>
        override protected void ChangeDriverPosition(byte carPosition, DriverData driverData, Session sessionData, int index)
        {
            //Update indexes for leader
            if (carPosition == 1)
                ChangeLeader(driverData, sessionData);

            _driverEntries[index].SetName(driverData.RaceNumber); //Set initals for that position
            _driverEntries[index].SetTeamColor(GameManager.F1Utility.GetColorByTeam(driverData.ParticipantData.team)); //Set team color

            //Set delta to leader if finished
            if (_deltaToLeaderFinish.ContainsKey(driverData.ID))
                _driverEntries[index].SetDeltaToLeader(_deltaToLeaderFinish[driverData.ID].IntervalToLeader);

            //if the car is retired, set it to out
            ResultStatus resultStatus = driverData.LapData.resultStatus;
            if (resultStatus == ResultStatus.Retired || resultStatus == ResultStatus.Disqualified)
                _driverEntries[index].Out(resultStatus);
            else
            {
                //If it was previously out make it not
                if (_driverEntries[index].OutOfSession)
                    _driverEntries[index].NotOut();

                //Change color wether driver GAINED or LOST to this position -> compare old position with this one
                _driverEntries[index].UpdatePositionColor(_driverPosition[driverData.ID], _movedUpColor, _movedDownColor);
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

            _leaderVehicleIndex = driverData.VehicleIndex;
        }

        #endregion

        #region Driver Update

        /// <summary>
        /// Sets the time text for a driver depending on distance to leader.
        /// </summary>
        void UpdateDriverTimingToLeader(DriverData leaderData, Session sessionData, DriverData driverData, bool inPit, int index)
        {
            float lapCompletion = LapCompletion(sessionData, driverData);
            float leaderLapCompletion = LapCompletion(sessionData, leaderData);
            int timingIndex = (int)(lapCompletion * (_amountOfTimingStations - 1));

            bool passedLeader = _timingData[timingIndex].PassedByLeader;

            //This is the leader!
            if (leaderData.VehicleIndex == driverData.VehicleIndex)
            {
                //It is leader so delta to itself is 0
                _driverEntries[index].SetDeltaToLeader(0);
                if (!inPit)
                    _driverEntries[index].SetTimingState(DriverTimeState.Leader);
            }
            else if (IsLapped(timingIndex, driverData, sessionData, out int amountOfLaps))
            {
                if (!inPit)
                    _driverEntries[index].SetTimingState(DriverTimeState.Lapped);
                _driverEntries[index].SetLapsLapped(amountOfLaps);
            }
            else if (!passedLeader && !inPit)
                _driverEntries[index].SetTimingState(DriverTimeState.Starting);
            //If nothing else -> Show delta
            else if (!inPit)
                _driverEntries[index].SetTimingState(DriverTimeState.Delta);

            //No need to update since it hasn't passed a new station
            if (_driverLastTimeSpot[driverData.ID] == timingIndex)
                return;
            //Update last time spot for this driver
            else
                _driverLastTimeSpot[driverData.ID] = timingIndex;

            //Index of current timing to compare with
            float currentTime = GameManager.F1Info.SessionTime;
            float deltaToLeader = currentTime - _timingData[timingIndex].Time;

            _driverEntries[index].SetDeltaToLeader(deltaToLeader);
        }

        /// <summary>
        /// Checks if driver is currently in pit and sets it so in entry
        /// </summary>
        override protected bool IsDriverInPit(DriverData driverData, int index)
        {
            if (driverData.LapData.pitStatus == PitStatus.Pitting)
                _driverEntries[index].SetTimingState(DriverTimeState.Pit);
            else if (driverData.LapData.pitStatus == PitStatus.In_Pit_Area)
                _driverEntries[index].SetTimingState(DriverTimeState.Pit_Area);
            else
                return false;
            return true;
        }

        /*OLD ISLAPPED FUNCTION
         * 
         * 
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
        */

        /// <summary>
        /// Determine if a car is lapped based on timing stations the leader have passed.
        /// </summary>
        /// <param name="amountOfLaps">How many laps the car is lapped if any</param>
        /// <returns>True if it is lapped</returns> 
        bool IsLapped(int timingIndex, DriverData driverData, Session sessionData, out int amountOfLaps)
        {
            //Compare one behind current as to not have it flicker
            if (timingIndex == 0)
                timingIndex = _amountOfTimingStations - 1;
            else
                timingIndex--;

            //Leader must have passed it to be able to determine
            if (_timingData[timingIndex].PassedByLeader)
            {
                int lapsBehind = _timingData[timingIndex].Lap - driverData.LapData.currentLapNumber;
                //Is lapped then
                if (lapsBehind > 0)
                {
                    amountOfLaps = lapsBehind;
                    return true;
                }
            }
            amountOfLaps = 0;
            return false;
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
}
