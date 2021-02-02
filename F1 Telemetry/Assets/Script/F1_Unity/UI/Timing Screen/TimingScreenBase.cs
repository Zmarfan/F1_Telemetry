using System.Collections.Generic;
using UnityEngine;
using F1_Data_Management;
using System.Linq;

namespace F1_Unity
{
    /// <summary>
    /// The base class which race and qualifying screen inherites from
    /// </summary>
    public abstract class TimingScreenBase : MonoBehaviour
    {
        #region Fields

        [SerializeField] protected TimingStats.TimingStatsState[] _availableStatsState;
        [SerializeField, Range(0, 5000)] protected float _initialsNameWidth = 480.2f;
        [SerializeField, Range(0, 5000)] protected float _fullNameWidth = 960f;
        [SerializeField, Range(0, 500)] int _initialsModeFontSize = 280;
        [SerializeField, Range(0, 500)] int _fullNameModeFontSize = 225;
        [SerializeField, Range(0.01f, 5f)] protected float _flashColorDuration = 1.0f;
        [SerializeField, Range(0.01f, 50f)] protected float _changeBackColorDuration = 4.5f;
        [SerializeField] protected Color _movedUpColor = Color.green;
        [SerializeField] protected Color _movedDownColor = Color.red;
        [SerializeField] protected CanvasGroup _canvasGroup;
        [SerializeField] protected TimingScreenEntry[] _driverEntries;

        //Reach driver position by their ID
        protected Dictionary<byte, int> _driverPosition;

        protected int _leaderVehicleIndex;
        protected bool _initValues = true;

        protected TimeScreenState _timeScreenState = TimeScreenState.Leader;
        /// <summary>
        /// Shows driver intials if true or full names if false
        /// </summary>
        protected bool _initialsMode;
        protected int _timingStatsStateIndex = 0;

        #endregion

        #region Start Init

        private void Awake()
        {
            Init();
        }

        /// <summary>
        /// Called on awake to initilize timing screen
        /// </summary>
        protected virtual void Init()
        {
            for (int i = 0; i < _driverEntries.Length; i++)
            {
                _driverEntries[i].Init(i + 1, _flashColorDuration, _changeBackColorDuration);
                _driverEntries[i].ChangeInitialMode(_initialsMode, _initialsNameWidth, _fullNameWidth, _initialsModeFontSize, _fullNameModeFontSize);
            }
        }

        protected abstract void InitDrivers();

        private void OnEnable()
        {
            GameManager.InputManager.PressedTimeInterval += ChangeTimingMode;
            GameManager.InputManager.PressedTimeStatsState += ChangeTimingStatsState;
            GameManager.InputManager.PressedTimingNameMode += ChangeDriverNameMode;
        }

        private void OnDisable()
        {
            GameManager.InputManager.PressedTimeInterval -= ChangeTimingMode;
            GameManager.InputManager.PressedTimeStatsState -= ChangeTimingStatsState;
            GameManager.InputManager.PressedTimingNameMode -= ChangeDriverNameMode;
        }

        #endregion

        #region Modes

        /// <summary>
        /// Changes betwen interval mode and to leader
        /// </summary>
        protected void ChangeTimingMode()
        {
            _timeScreenState = (TimeScreenState)(((int)_timeScreenState + 1) % (int)TimeScreenState.Length);
            SetMode(_timeScreenState);
        }

        /// <summary>
        /// Changes to show driver initials or full name (resize for full name, downsize for initials)
        /// </summary>
        protected void ChangeDriverNameMode()
        {
            _initialsMode = !_initialsMode;
            for (int i = 0; i < _driverEntries.Length; i++)
                _driverEntries[i].ChangeInitialMode(_initialsMode, _initialsNameWidth, _fullNameWidth, _initialsModeFontSize, _fullNameModeFontSize);
        }

        /// <summary>
        /// Sets mode to interval or to leader
        /// </summary>
        protected virtual void SetMode(TimeScreenState timeScreenState)
        {
            for (int i = 0; i < _driverEntries.Length; i++)
                _driverEntries[i].SetMode(timeScreenState);
        }

        /// <summary>
        /// Called when changing timing stats state -> changes all entries to that state
        /// </summary>
        protected void ChangeTimingStatsState()
        {
            _timingStatsStateIndex = (_timingStatsStateIndex + 1) % _availableStatsState.Length;
            SetStatsState(_availableStatsState[_timingStatsStateIndex]);
        }

        /// <summary>
        /// Updates all templates to this stats state
        /// </summary>
        protected virtual void SetStatsState(TimingStats.TimingStatsState state)
        {
            for (int i = 0; i < _driverEntries.Length; i++)
                _driverEntries[i].SetStatsState(state);
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Get the driver template for specific index (Used to read delta for each driver)
        /// </summary>
        /// <param name="index">Position - 1</param>
        /// <returns>Access to a driver's delta, state and info</returns>
        public TimingScreenEntry GetDriverTemplate(int index)
        {
            return _driverEntries[index];
        }

        /// <summary>
        /// Shows or not show timing screen (will still be active in background)
        /// </summary>
        public void SetActive(bool status)
        {
            _canvasGroup.alpha = status ? 1.0f : 0.0f;
        }

        /// <summary>
        /// Completely resets to original prefab state -> used when changing between sessions
        /// </summary>
        public virtual void CompleteReset()
        {
            _driverPosition = null;
            _initValues = true;

            _timeScreenState = TimeScreenState.Leader;
            _timingStatsStateIndex = 0;

            Init();
        }

        #endregion

        #region Update Methods

        //Updates standing
        public void UpdateTimingScreen()
        {
            //Only update standings when data can be read safely and correctly
            if (GameManager.F1Info.ReadyToReadFrom)
            {
                if (_initValues)
                    InitDrivers();
                else
                    MainUpdate();
            }
        }

        /// <summary>
        /// Does all updating for timing screen. Called once per frame.
        /// </summary>
        protected abstract void MainUpdate();

        /// <summary>
        /// Sets all aspect of timing for specific driver
        /// </summary>
        protected abstract void DoDriver(DriverData driverData, DriverData leaderData, Session sessionData);

        /// <summary>
        /// //Set time text for each template and calculate interval based on DeltaToLeader
        /// </summary>
        protected void CalculateDriverIntervals()
        {
            _driverEntries[0].SetTiming(); //Leader
            for (int i = 1; i < _driverEntries.Length; i++)
            {
                float previousCarDeltaToLeader = _driverEntries[i - 1].DeltaToLeader;
                _driverEntries[i].SetCarAheadDelta(previousCarDeltaToLeader);
                _driverEntries[i].SetTiming();
            }
        }

        #endregion

        #region Positioning

        /// <summary>
        /// Takes care of positioning and coloring of overtakes
        /// </summary>
        protected void Positioning(DriverData driverData, Session sessionData, int index)
        {
            byte carPosition = driverData.LapData.carPosition;

            //If this driver doesn't exist, it has just joined the session, recalculate everything!
            if (!_driverPosition.ContainsKey(driverData.ID))
                InitDrivers();

            //Drivers position has changed! Update!
            if (_driverPosition[driverData.ID] != carPosition)
                ChangeDriverPosition(carPosition, driverData, sessionData, index);
        }

        /// <summary>
        /// Change driver position and exchange values between templates -> update leader values if leader changed
        /// </summary>
        protected abstract void ChangeDriverPosition(byte carPosition, DriverData driverData, Session sessionData, int index);

        /// <summary>
        /// Copy color values between two entries (done in overtakes to make event color stick to driver)
        /// </summary>
        protected virtual void ChangeColorData(TimingScreenEntry entry1, TimingScreenEntry entry2)
        {
            entry1.GetColorValues(out Color currentFromDarkColor1, out Color currentFromLightColor1, out Color currentDarkColor1, out Color currentLightColor1,  out float colorTime1);
            entry2.GetColorValues(out Color currentFromDarkColor2, out Color currentFromLightColor2, out Color currentDarkColor2, out Color currentLightColor2, out float colorTime2);
            entry1.SetColorValues(currentFromDarkColor2, currentFromLightColor2, currentDarkColor2, currentLightColor2, colorTime2);
            entry2.SetColorValues(currentFromDarkColor1, currentFromLightColor1, currentDarkColor1, currentLightColor1, colorTime1);
        }

        #endregion

        #region Driver Update

        /// <summary>
        /// Checks if driver is currently in pit and sets it so in entry
        /// </summary>
        protected abstract bool IsDriverInPit(DriverData driverData, int index);

        #endregion

        #region Help Methods

        /// <summary>
        /// How far along a lap is a driver? (0.0f - 1.0f)
        /// </summary>
        protected float LapCompletion(Session sessionData, DriverData driverData)
        {
            return Mathf.Clamp01(driverData.LapData.lapDistance / sessionData.TrackLength);
        }

        /// <summary>
        /// Returns current lap for a driver clamped to amount of lap in session
        /// </summary>
        /// <returns></returns>
        protected int GetCurrentLapClamped(DriverData driverData, Session sessionData)
        {
            return Mathf.Clamp(driverData.LapData.currentLapNumber, 0, sessionData.TotalLaps);
        }

        /// <summary>
        /// Return the index for template based on driver position
        /// </summary>
        protected int GetEntryIndex(DriverData driverData)
        {
            return driverData.LapData.carPosition - 1;
        }

        /// <summary>
        /// Better modulo than % with only giving positive values
        /// </summary>
        protected int Modulo(int x, int m)
        {
            int r = x % m;
            return r < 0 ? r + m : r;
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