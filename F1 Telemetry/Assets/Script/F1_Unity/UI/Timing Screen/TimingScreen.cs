using System.Collections.Generic;
using UnityEngine;
using F1_Data_Management;

namespace F1_Unity
{
    public class TimingScreen : MonoBehaviour
    {
        public static readonly float LEADER_LAP_EPSILON = 0.05f;

        [SerializeField, Range(10, 400)] int _amountOfTimingStations = 100;
        [SerializeField, Range(0.01f, 5f)] float _flashColorDuration = 1.0f;
        [SerializeField, Range(0.01f, 50f)] float _changeBackColorDuration = 4.5f;
        [SerializeField] UnityEngine.Color _movedUpColor = UnityEngine.Color.green;
        [SerializeField] UnityEngine.Color _movedDownColor = UnityEngine.Color.red;
        [SerializeField] DriverTemplate[] _driverTemplates;

        static TimingScreen _singleton;

        //Reach driver position by their ID
        Dictionary<byte, int> _driverPosition;
        Dictionary<byte, int> _driverLastTimeSpot;
        DriverTimingData[] _timingData;
        int _leaderVehicleIndex;
        int _lastPassedTimingIndex;
        bool _initValues = true;

        bool _intervalMode = false;

        private void Awake()
        {
            if (_singleton == null)
                Init();
            else
                Destroy(this.gameObject);
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
        /// Changes betwen interval mode and to leader
        /// </summary>
        void ChangeTimingMode()
        {
            _intervalMode = !_intervalMode;
            SetMode(_intervalMode);
        }

        /// <summary>
        /// Assign correct number to each placement and create singleton
        /// </summary>
        void Init()
        {
            _singleton = this;
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

            _timingData = new DriverTimingData[_amountOfTimingStations];

            _driverPosition = new Dictionary<byte, int>();
            _driverLastTimeSpot = new Dictionary<byte, int>();
            Session sessionData = GameManager.F1Info.ReadSession(out bool status);

            for (int i = 0; i < F1Info.MAX_AMOUNT_OF_CARS; i++)
            {
                DriverData driverData = GameManager.F1Info.ReadCarData(i, out bool validDriver);

                //Init everyone with gaining a position on start!
                //Flash green then to white
                //Only add valid drivers to the grid
                if (validDriver && status)
                {
                    _driverPosition.Add(driverData.ID, driverData.LapData.carPosition + 1);
                    _driverTemplates[i].UpdateTimingValues(DriverTimeState.Starting);
                    _driverTemplates[i].SetTiming();

                    //If the car is already retired -> set it so
                    ResultStatus resultStatus = driverData.LapData.resultStatus;
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
                //if (i >= GameManager.F1Info.ActiveDrivers)
                //    _driverTemplates[i].SetActive(false);
                //else
                //    _driverTemplates[i].SetActive(true);

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

        /// <summary>
        /// Sets mode to interval or to leader
        /// </summary>
        void SetMode(bool interval)
        {
            for (int i = 0; i < _driverTemplates.Length; i++)
                _driverTemplates[i].SetMode(interval);
        }

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
        /// Updates positions in standing and checks stability
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

                _driverTemplates[positionIndex].SetInitials(driverData.ParticipantData.driverInitial); //Set initals for that position
                _driverTemplates[positionIndex].SetTeamColor(driverData.ParticipantData.teamColor); //Set team color

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

            //This is the leader!
            if (leaderData.VehicleIndex == driverData.VehicleIndex)
            {
                _driverTemplates[leaderData.LapData.carPosition - 1].UpdateTimingValues(DriverTimeState.Leader);
                return;
            }

            int lapsBehind = leaderData.LapData.currentLapNumber - driverData.LapData.currentLapNumber;

            //Sometimes values around 0.98 < value < 1.0 might still be on a new lap, add epsilon and overflow  
            leaderLapCompletion += LEADER_LAP_EPSILON;
            leaderLapCompletion = leaderLapCompletion > 1 ? leaderLapCompletion - 1 : leaderLapCompletion;

            //Could be that this car hasn't crossed finish line yet!
            if (lapCompletion > leaderLapCompletion)
                lapsBehind--;

            //This car is lapped
            if (lapsBehind > 0)
            {
                Debug.Log("LAPPED");
                _driverTemplates[driverData.LapData.carPosition - 1].UpdateTimingValues(DriverTimeState.Lapped, 0, lapsBehind);
                return;
            }

            //Index of current timing to compare with
            float currentTime = GameManager.F1Info.SessionTime;
            float deltaToLeader = currentTime - _timingData[timingIndex].Time;
            DriverTimeState state = _timingData[timingIndex].PassedByLeader ? DriverTimeState.Delta : DriverTimeState.Starting;

            _driverTemplates[driverData.LapData.carPosition - 1].UpdateTimingValues(state, deltaToLeader);
        }

        /// <summary>
        /// How far along a lap is a driver? (0.0f - 1.0f)
        /// </summary>
        float LapCompletion(Session sessionData, DriverData driverData)
        {
            return Mathf.Clamp01(driverData.LapData.lapDistance / sessionData.TrackLength);
        }

        /// <summary>
        /// A timing station that holds time and lap when leader passed
        /// </summary>
        struct DriverTimingData
        {
            public bool PassedByLeader { get; set; }
            public byte Lap { get; set; }
            public float Time { get; set; }
        }
    }
}
