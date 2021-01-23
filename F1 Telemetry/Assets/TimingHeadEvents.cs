using UnityEngine;
using UnityEngine.UI;
using F1_Data_Management;
using System.Text;
using System.Collections.Generic;

namespace F1_Unity
{
    /// <summary>
    /// Shows flag info, laps to go, final lap on top of timing screen
    /// </summary>
    public class TimingHeadEvents : MonoBehaviour
    {
        [Header("Affected UI")]

        [SerializeField, Tooltip("UI that gets colored to flag color on info shown")] AffectedByFlagElement[] _affectedUI;

        [Header("Settings")]

        [SerializeField, Range(1, 20)] int _lapsToGoFrequency = 5;
        [SerializeField, Range(0.01f, 100)] float _showInfoTime = 5f;
        [SerializeField] string _greenFlagString = "TRACK CLEAR";
        [SerializeField] string _sectorString = "SECTOR ";
        [SerializeField] Color _lastLapColor;
        [SerializeField] Color _lapsToGoColor;
        [SerializeField] Color _greenFlagColor;
        [SerializeField] Color _yellowFlagColor;

        [Header("Drop")]

        [SerializeField] GameObject _activeObj;
        [SerializeField] Image _mainImage;
        [SerializeField] GameObject[] _statesObj;
        [SerializeField] Text _flagText;
        [SerializeField] Text _lapsToGoText;

        static readonly int FINAL_LAP_INDEX = 0;
        static readonly int FLAG_INDEX = 1;
        static readonly int LAPS_TO_GO_INDEX = 2;

        Timer _showInfoTimer;
        int _lastLapsLeftLap = 0;
        bool _doneFinalLap = false;
        bool _showingInfo = false;
        bool _up = false;
        bool _wasYellowFlag = false;

        private void Awake()
        {
            _showInfoTimer = new Timer(_showInfoTime);
        }

        private void Update()
        {
            Session sessionData = GameManager.F1Info.ReadSession(out bool sessionStatus);
            DriverData leaderData = GameManager.DriverDataManager.GetDriverFromPosition(1, out bool status);

            //Done only as to reset done final lap on session restart if race restart
            if (leaderData.LapData.currentLapNumber < sessionData.TotalLaps)
                _doneFinalLap = false;

            if (sessionStatus && status)
            {
                CheckFlags(sessionData);
                CheckEvents(leaderData, sessionData);
            }
            //Don't show anything
            else
                _up = false;

            UpdateTimers();
            _activeObj.SetActive(_up);
        }

        /// <summary>
        /// Turn off all except active
        /// </summary>
        void SetActiveState(GameObject active)
        {
            for (int i = 0; i < _statesObj.Length; i++)
                _statesObj[i].SetActive(_statesObj[i] == active);
        }

        /// <summary>
        /// Update uptime timers for info and closes info when timer expire
        /// </summary>
        void UpdateTimers()
        {
            if (_showingInfo)
            {
                //Keep info up while timer hasn't expired
                _showInfoTimer.Time += Time.deltaTime;
                //Remove event
                if (_showInfoTimer.Expired())
                {
                    _showInfoTimer.Reset();
                    SetAffectedUIColor(AffectedUIState.Normal);
                    SetActiveState(null); //Turn off all
                    _showingInfo = false;
                    _up = false;
                }
            }
        }

        /// <summary>
        /// Activate flag mode if no other info is displayed if there are flag waved
        /// </summary>
        void CheckFlags(Session sessionData)
        {
            //Show info as long as it is there, no timer for flags
            if (!_showingInfo)
            {
                //True if yellow flag in that zone
                bool[] sectors = new bool[3];
                bool yellow = false;
                for (int i = 0; i < sessionData.NumberOfMarshalZones; i++)
                {
                    MarshalZone zone = sessionData.MarshalZones[i];
                    CheckSector(zone, ref sectors, ref yellow);
                }

                //Just turned into green flag conditions! Start info about green
                if (!yellow && _wasYellowFlag)
                    StartGreenFlag();
                else if (yellow)
                    YellowFlag(sectors);
            }
        }

        /// <summary>
        /// Checks if there is yellow flag at zone and if there is assign true to that sector
        /// </summary>
        void CheckSector(MarshalZone zone, ref bool[] sectors, ref bool yellow)
        {
            if (zone.zoneFlag == Flag.Yellow)
            {
                int sector = (int)(zone.zoneStart * 2.9999f);
                sectors[sector] = true;
                yellow = true;
            }
        }

        /// <summary>
        /// Activate yellow flag state and write correct sectors
        /// </summary>
        /// <param name="sectorStates"></param>
        void YellowFlag(bool[] sectorStates)
        {
            _flagText.text = GetYellowFlagString(sectorStates);
            _up = true;
            _wasYellowFlag = true;
            _mainImage.color = _yellowFlagColor;

            SetActiveState(_statesObj[FLAG_INDEX]);
        }

        /// <summary>
        /// Sets the color of affected UI (UI that should change color with flag state)
        /// </summary>
        void SetAffectedUIColor(AffectedUIState state)
        {
            for (int i = 0; i < _affectedUI.Length; i++)
            {
                Color changeColor;
                switch (state)
                {
                    case AffectedUIState.Yellow: changeColor = _yellowFlagColor; break;
                    case AffectedUIState.Green: changeColor = _greenFlagColor; break;
                    case AffectedUIState.Normal: changeColor = _affectedUI[i].changeBackColor; break;
                    default: throw new System.Exception("There is no implementation to handle: " + state);
                }
                _affectedUI[i].graphic.color = changeColor;
            }
        }

        /// <summary>
        /// Creates a string to display at yellow flag info that displays correct sectors
        /// </summary>
        string GetYellowFlagString(bool[] sectorStates)
        {
            //Numbers of sector that have yellow flag in order
            List<int> sectors = new List<int>();
            for (int i = 0; i < sectorStates.Length; i++)
            {
                if (sectorStates[i])
                    sectors.Add(i + 1);
            }

            StringBuilder builder = new StringBuilder();
            builder.Append(_sectorString);
            builder.Append(sectors[0]);
            //2 yellow sectors -> & x
            if (sectors.Count == 2)
            {
                builder.Append(" & ");
                builder.Append(sectors[1]);
            }
            //3 yellow sectors 1, 2 & 3
            else if (sectors.Count == 3)
            {
                builder.Append(", ");
                builder.Append(sectors[1]);
                builder.Append(" & ");
                builder.Append(sectors[2]);
            }

            return builder.ToString();
        }

        /// <summary>
        /// Check for events and start them if possible
        /// </summary>
        void CheckEvents(DriverData leaderData, Session sessionData)
        {
            //Last lap and not done yet
            if (leaderData.LapData.currentLapNumber == sessionData.TotalLaps && !_doneFinalLap && !_showingInfo)
                StartLastLap();

            bool lapUpdateTime = ((sessionData.TotalLaps - leaderData.LapData.currentLapNumber + 1) % _lapsToGoFrequency == 0) && leaderData.LapData.currentLapNumber <= sessionData.TotalLaps && !_showingInfo;
            //Laps to go display time
            if (lapUpdateTime && _lastLapsLeftLap != leaderData.LapData.currentLapNumber)
            {
                _lastLapsLeftLap = leaderData.LapData.currentLapNumber;
                StartDisplayLapsToGo(sessionData.TotalLaps -  leaderData.LapData.currentLapNumber + 1);
            }
        }

        /// <summary>
        /// Activate green flag info and start timer for it's uptime -> set color and string
        /// </summary>
        void StartGreenFlag()
        {
            _wasYellowFlag = false;
            _flagText.text = _greenFlagString;
            SetAffectedUIColor(AffectedUIState.Green);
            StartInfo(FLAG_INDEX, _greenFlagColor);
        }

        /// <summary>
        /// Activate last lap info and start timer for it's uptime -> set color aswell
        /// </summary>
        void StartLastLap()
        {
            _doneFinalLap = true;
            SetAffectedUIColor(AffectedUIState.Normal);
            StartInfo(FINAL_LAP_INDEX, _lastLapColor);
        }

        /// <summary>
        /// Activate laps to go info and start timer for it's uptime -> set colors and info
        /// </summary>
        void StartDisplayLapsToGo(int lapsToGo)
        {
            _lapsToGoText.text = lapsToGo.ToString();
            SetAffectedUIColor(AffectedUIState.Normal);
            StartInfo(LAPS_TO_GO_INDEX, _lapsToGoColor);
        }

        /// <summary>
        /// Activate correct state and assign bool to indicate info is active
        /// </summary>
        void StartInfo(int stateIndex, Color mainColor)
        {
            _mainImage.color = mainColor;
            _showingInfo = true;
            _up = true;
            SetActiveState(_statesObj[stateIndex]);
        }

        enum AffectedUIState
        {
            Yellow,
            Green,
            Normal,
        }

        [System.Serializable]
        public struct AffectedByFlagElement
        {
            public Graphic graphic;
            public Color changeBackColor;
        }
    }
}