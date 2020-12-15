using System.Collections.Generic;
using UnityEngine;
using F1_Data_Management;

namespace F1_Unity
{
    public class TimingScreen : MonoBehaviour
    {
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
        TimingStation[] _timingData;
        int _leaderVehicleIndex;
        int _lastPassedTimingIndex;
        bool _initValues = true;

        bool _intervalMode = false;

        #region Init

        private void Awake()
        {
            Init();
        }

        private void OnEnable()
        {
            InputManager.PressedTimeInterval += ChangeTimingMode;
        }

        private void OnDisable()
        {
            InputManager.PressedTimeInterval -= ChangeTimingMode;
        }

        /// <summary>
        /// Shows or not show timing screen (will still be active in background)
        /// </summary>
        public void SetActive(bool status)
        {
            _canvasGroup.alpha = status ? 1.0f : 0.0f;
        }

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

            SetMode(_intervalMode);

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
                    _driverTemplates[driverData.LapData.carPosition - 1].SetDriverData(driverData);

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
                if (resultStatus == ResultStatus.Inactive || resultStatus == ResultStatus.Invalid)
                    _driverTemplates[i].SetActive(false);
                else
                    _driverTemplates[i].SetActive(true);
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
            _intervalMode = !_intervalMode;
            SetMode(_intervalMode);
        }

        /// <summary>
        /// Sets mode to interval or to leader
        /// </summary>
        void SetMode(bool interval)
        {
            for (int i = 0; i < _driverTemplates.Length; i++)
                _driverTemplates[i].SetMode(interval);
        }

        #endregion

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

        #region Calculate

        /// <summary>
        /// Updates positions in standing and checks stability, also updates driver stats
        /// </summary>
        void DoTimingScreen()
        {
            //Will be valid -> checked earlier
            Session sessionData = GameManager.F1Info.ReadSession(out bool status);
            DriverData leaderData = GameManager.F1Info.ReadCarData(_leaderVehicleIndex, out bool status2);
            UpdateLeader(sessionData, leaderData);

            //Loop through all drivers
            for (int i = 0; i < F1Info.MAX_AMOUNT_OF_CARS; i++)
            {
                bool validDriver;
                DriverData driverData = GameManager.F1Info.ReadCarData(i, out validDriver);
                //Skip the drivers that have no valid index -> junk data
                if (!validDriver)
                    continue;

                //Sets driverdata if it's usage is needed and to update timing 
                _driverTemplates[driverData.LapData.carPosition - 1].SetDriverData(driverData);
                //Update stats for driver
                _driverTemplates[driverData.LapData.carPosition - 1].UpdateStats(driverData);
                Positioning(driverData);
                UpdateDriverTimingToLeader(leaderData, sessionData, driverData);
            }

            //Set time text for each template and calculate interval
            _driverTemplates[0].SetTiming();
            for (int i = 1; i < _driverTemplates.Length; i++)
            {
                float previousCarDeltaToLeader = _driverTemplates[i - 1].DeltaToLeader;
                _driverTemplates[i].SetCarAheadDelta(previousCarDeltaToLeader);
                _driverTemplates[i].SetTiming();
            }
        }

        /// <summary>
        /// Saves time and lap leader passed a timing station for other cars to be able to compare with.
        /// </summary>
        void UpdateLeader(Session sessionData, DriverData leaderData)
        {
            //Find closest time station
            float lapCompletion = LapCompletion(sessionData, leaderData);
            int timingIndex = (int)(lapCompletion * (_amountOfTimingStations - 1));
            //It's a new time station
            if (timingIndex != _lastPassedTimingIndex)
            {
                _timingData[timingIndex].Time = GameManager.F1Info.SessionTime;
                _timingData[timingIndex].Lap = leaderData.LapData.currentLapNumber;
                _timingData[timingIndex].PassedByLeader = true;
                _lastPassedTimingIndex = timingIndex;
            }
        }

        /// <summary>
        /// Takes care of positioning and coloring of overtakes
        /// </summary>
        void Positioning(DriverData driverData)
        {
            byte driverID = driverData.ID;
            byte carPosition = driverData.LapData.carPosition;

            //If this driver doesn't exist, it has just joined the session, recalculate everything!
            if (!_driverPosition.ContainsKey(driverID) || !_driverLastTimeSpot.ContainsKey(driverID))
                InitDrivers();

            //Drivers position has changed! Update!
            if (_driverPosition[driverID] != carPosition)
            {
                int positionIndex = carPosition - 1; //Index in array is always one less than position

                //Update leader driverIndex
                if (positionIndex == 0)
                    _leaderVehicleIndex = driverData.ParticipantData.vehicleIndex;

                _driverTemplates[positionIndex].SetInitials(GameManager.ParticipantManager.GetDriverInitials(driverData.RaceNumber)); //Set initals for that position
                _driverTemplates[positionIndex].SetTeamColor(GameManager.F1Utility.GetColorByTeam(driverData.ParticipantData.team)); //Set team color

                //if the car is retired, set it to out
                ResultStatus resultStatus = driverData.LapData.resultStatus;
                if (resultStatus == ResultStatus.Retired || resultStatus == ResultStatus.Disqualified)
                    _driverTemplates[positionIndex].Out(resultStatus);
                else
                {
                    //If it was previously out make it not
                    if (_driverTemplates[positionIndex].OutOfSession)
                        _driverTemplates[positionIndex].NotOut();

                    //Change color wether driver GAINED or LOST to this position -> compare old position with this one
                    _driverTemplates[positionIndex].UpdatePositionColor(_driverPosition[driverID], _movedUpColor, _movedDownColor);
                }

                //save this position to compare in future
                _driverPosition[driverID] = carPosition;
            }
        }

        /// <summary>
        /// Sets the time text for a driver depending on distance to leader.
        /// </summary>
        void UpdateDriverTimingToLeader(DriverData leaderData, Session sessionData, DriverData driverData)
        {
            int index = driverData.LapData.carPosition - 1;

            //Pits
            if (driverData.LapData.pitStatus == PitStatus.Pitting)
                _driverTemplates[index].SetTimingState(DriverTimeState.Pit);
            else if (driverData.LapData.pitStatus == PitStatus.In_Pit_Area)
                _driverTemplates[index].SetTimingState(DriverTimeState.Pit_Area);

            float lapCompletion = LapCompletion(sessionData, driverData);
            float leaderLapCompletion = LapCompletion(sessionData, leaderData);
            int timingIndex = (int)(lapCompletion * (_amountOfTimingStations - 1));

            byte driverID = driverData.ID;
            //No need to update since it hasn't passed a new station
            if (_driverLastTimeSpot[driverID] == timingIndex)
                return;
            //Update last time spot for this driver
            else
                _driverLastTimeSpot[driverID] = timingIndex;

            //Index of current timing to compare with
            float currentTime = GameManager.F1Info.SessionTime;
            float deltaToLeader = currentTime - _timingData[timingIndex].Time;

            _driverTemplates[index].SetDeltaToLeader(deltaToLeader);
            bool passedLeader = _timingData[timingIndex].PassedByLeader;

            //This is the leader!
            if (leaderData.VehicleIndex == driverData.VehicleIndex)
                _driverTemplates[index].SetTimingState(DriverTimeState.Leader);
            else if (IsLapped(leaderLapCompletion, lapCompletion, leaderData, driverData, out int amountOfLaps))
            {
                //It's lapped and in Leader mode -> show laps
                if (!_intervalMode)
                {
                    _driverTemplates[index].SetTimingState(DriverTimeState.Lapped);
                    _driverTemplates[index].SetLapsLapped(amountOfLaps);
                }
            }
            //If nothing else -> Show delta
            else if (!passedLeader)
                _driverTemplates[index].SetTimingState(DriverTimeState.Starting);
            else
                _driverTemplates[index].SetTimingState(DriverTimeState.Delta);

        }

        /// <summary>
        /// How far along a lap is a driver? (0.0f - 1.0f)
        /// </summary>
        float LapCompletion(Session sessionData, DriverData driverData)
        {
            return Mathf.Clamp01(driverData.LapData.lapDistance / sessionData.TrackLength);
        }

        bool IsLapped(float leaderLapCompletion, float lapCompletion, DriverData leaderData, DriverData driverData, out int amountOfLaps)
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

        /// <summary>
        /// A timing station that holds time and lap when leader passed
        /// </summary>
        [System.Serializable]
        struct TimingStation
        {
            public bool PassedByLeader { get; set; }
            public byte Lap { get; set; }
            public float Time { get; set; }
        }
    }
}
