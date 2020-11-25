using System.Collections.Generic;
using UnityEngine;
using F1_Data_Management;
using System;
using System.Text;

namespace F1_Unity
{
    public class TimingScreen : MonoBehaviour
    {
        [SerializeField, Range(10, 400)] int _amountOfTimingStations = 100;
        [SerializeField, Range(0.01f, 5f)] float _flashColorDuration = 1.0f;
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

        private void Awake()
        {
            if (_singleton == null)
                Init();
            else
                Destroy(this.gameObject);
        }

        /// <summary>
        /// Assign correct number to each placement and create singleton
        /// </summary>
        void Init()
        {
            _singleton = this;
            for (int i = 0; i < _driverTemplates.Length; i++)
                _driverTemplates[i].Init(i + 1, _flashColorDuration);
        }


        /// <summary>
        /// Maps driver IDs to position on track. Used to compare old position to current
        /// </summary>
        void InitDrivers()
        {
            _initValues = false;

            _timingData = new DriverTimingData[_amountOfTimingStations];

            _driverPosition = new Dictionary<byte, int>();
            _driverLastTimeSpot = new Dictionary<byte, int>();

            for (int i = 0; i < F1Info.MAX_AMOUNT_OF_CARS; i++)
            {
                DriverData driverData = GameManager.F1Info.ReadCarData(i, out bool validDriver);
                Session sessionData = GameManager.F1Info.ReadSession(out bool status);

                //Init everyone with gaining a position on start!
                //Flash green then to white
                //Only add valid drivers to the grid
                if (validDriver && status)
                {
                    _driverPosition.Add(driverData.ParticipantData.driverID, driverData.LapData.carPosition + 1);

                    float lapCompletion = LapCompletion(sessionData, driverData);
                    int timingIndex = (int)(lapCompletion * (_amountOfTimingStations - 1));
                    _driverLastTimeSpot.Add(driverData.ParticipantData.driverID, timingIndex);

                    //Init leader
                    if (driverData.LapData.carPosition == 1)
                    {
                        _leaderVehicleIndex = driverData.VehicleIndex;
                        _lastPassedTimingIndex = timingIndex;
                    }
                }

                //Only enable so many positions in time standing as there are active drivers
                //If they DNF/DSQ later they will only gray out, not be removed
                if (i >= GameManager.F1Info.ActiveDrivers)
                    _driverTemplates[i].SetActive(false);
                else
                    _driverTemplates[i].SetActive(true);
            }
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
                SetTimingText(leaderData, sessionData, driverData);
            }
        }

        void UpdateLeader(Session sessionData, DriverData leaderData)
        {
            float lapCompletion = LapCompletion(sessionData, leaderData);
            int timingIndex = (int)(lapCompletion * (_amountOfTimingStations - 1));

            if (timingIndex != _lastPassedTimingIndex)
            {
                _timingData[timingIndex].Time = GameManager.F1Info.SessionTime;
                _timingData[timingIndex].Lap = leaderData.LapData.currentLapNumber;
                _lastPassedTimingIndex = timingIndex;
            }
        }

        /// <summary>
        /// Takes care of positioning and coloring of overtakes
        /// </summary>
        void Positioning(DriverData driverData)
        {
            byte driverID = driverData.ParticipantData.driverID;
            byte carPosition = driverData.LapData.carPosition;

            //If this driver doesn't exist, it has just joined the session, recalculate everything!
            if (!_driverPosition.ContainsKey(driverID) || !_driverLastTimeSpot.ContainsKey(driverID))
                Init();

            //Drivers position has changed! Update!
            if (_driverPosition[driverID] != carPosition)
            {
                int positionIndex = carPosition - 1; //Index in array is always one less than position

                //Update leader driverIndex
                if (positionIndex == 0)
                    _leaderVehicleIndex = driverData.ParticipantData.vehicleIndex;

                _driverTemplates[positionIndex].SetInitials(driverData.ParticipantData.driverInitial); //Set initals for that position

                //Change color wether driver GAINED or LOST to this position -> compare old position with this one
                _driverTemplates[positionIndex].UpdatePositionColor(_driverPosition[driverID], _movedUpColor, _movedDownColor);
                _driverTemplates[positionIndex].SetTeamColor(driverData.ParticipantData.teamColor); //Set team color

                //save this position to compare in future
                _driverPosition[driverID] = carPosition;
            }
        }

        void SetTimingText(DriverData leaderData, Session sessionData, DriverData driverData)
        {
            float lapCompletion = LapCompletion(sessionData, driverData);
            float leaderLapCompletion = LapCompletion(sessionData, leaderData);
            int timingIndex = (int)(lapCompletion * (_amountOfTimingStations - 1));

            byte driverID = driverData.ParticipantData.driverID;
            //No need to update since it hasn't passed a new station
            if (_driverLastTimeSpot[driverID] == timingIndex)
                return;
            else
                _driverLastTimeSpot[driverID] = timingIndex;

            //This is the leader!
            if (leaderData.VehicleIndex == driverData.VehicleIndex)
            {
                _driverTemplates[leaderData.LapData.carPosition - 1].SetTimeText("Leader");
                return;
            }

            int lapsBehind = leaderData.LapData.currentLapNumber - driverData.LapData.currentLapNumber;
            //Could be that this car hasn't crossed finish line yet!
            if (lapCompletion > leaderLapCompletion)
                lapsBehind--;

            //This car is lapped
            if (lapsBehind > 0)
            {
                _driverTemplates[driverData.LapData.carPosition - 1].SetTimeText("+" + lapsBehind + " LAP");
                return;
            }

            //Index of current timing to compare with
            float currentTime = GameManager.F1Info.SessionTime;
            float deltaToLeader = currentTime - _timingData[timingIndex].Time;

            string text = GetDeltaString(deltaToLeader);
            _driverTemplates[driverData.LapData.carPosition - 1].SetTimeText(text);
        }

        float LapCompletion(Session sessionData, DriverData driverData)
        {
            return Mathf.Clamp01(driverData.LapData.lapDistance / sessionData.TrackLength);
        }

        string GetDeltaString(float time)
        {
            TimeSpan span = TimeSpan.FromSeconds(time);
            StringBuilder builder = new StringBuilder();
            builder.Append('+');
            if (span.Minutes > 0)
            {
                builder.Append(span.Minutes);
                builder.Append(':');
                //Add a zero in front of seconds if it's one digit
                if (span.Seconds < 10)
                    builder.Append(0);
            }
            builder.Append(span.Seconds);
            builder.Append('.');

            int millieSeconds = (int)((time - (int)time) * 1000);
            builder.Append(millieSeconds);

            //Get zeroes
            if (millieSeconds == 0)
                builder.Append("000");
            else
            {
                while (millieSeconds < 100)
                {
                    builder.Append(0);
                    millieSeconds *= 10;
                }
            }

            return builder.ToString();
        }

        /// <summary>
        /// A timing station that holds time and lap when leader passed
        /// </summary>
        struct DriverTimingData
        {
            public byte Lap { get; set; }
            public float Time { get; set; }
        }
    }
}
