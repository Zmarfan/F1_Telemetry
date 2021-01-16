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

        [SerializeField, Range(0.01f, 5f)] protected float _flashColorDuration = 1.0f;
        [SerializeField, Range(0.01f, 50f)] protected float _changeBackColorDuration = 4.5f;
        [SerializeField] protected Color _movedUpColor = Color.green;
        [SerializeField] protected Color _movedDownColor = Color.red;
        [SerializeField] protected CanvasGroup _canvasGroup;
        [SerializeField] protected DriverTemplate[] _driverTemplates;

        //Reach driver position by their ID
        protected Dictionary<byte, int> _driverPosition;

        protected int _leaderVehicleIndex;
        protected bool _initValues = true;

        protected TimeScreenState _timeScreenState = TimeScreenState.Leader;
        protected TimingStats.TimingStatsState _timingStatsState = TimingStats.TimingStatsState.None;

        #endregion

        #region Start Init

        private void Awake()
        {
            Init();
        }

        protected abstract void Init();
        protected abstract void InitDrivers();

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
        /// Sets mode to interval or to leader
        /// </summary>
        protected virtual void SetMode(TimeScreenState timeScreenState)
        {
            for (int i = 0; i < _driverTemplates.Length; i++)
                _driverTemplates[i].SetMode(timeScreenState);
        }

        /// <summary>
        /// Called when changing timing stats state -> changes all templates to that state
        /// </summary>
        protected void ChangeTimingState()
        {
            _timingStatsState = (TimingStats.TimingStatsState)(((int)_timingStatsState + 1) % (int)TimingStats.TimingStatsState.Length);
            SetStatsState(_timingStatsState);
        }

        /// <summary>
        /// Updates all templates to this stats state
        /// </summary>
        protected virtual void SetStatsState(TimingStats.TimingStatsState state)
        {
            for (int i = 0; i < _driverTemplates.Length; i++)
                _driverTemplates[i].SetStatsState(state);
        }

        #endregion

        #region Public methods

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
            _timingStatsState = TimingStats.TimingStatsState.None;

            Init();
        }

        #endregion

        #region Update Methods

        //Updates standing
        private void Update()
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

        #endregion

        #region Driver Update

        /// <summary>
        /// Checks if driver is currently in pit and sets it so 
        /// </summary>
        protected bool IsDriverInPit(DriverData driverData, int index)
        {
            if (driverData.LapData.pitStatus == PitStatus.Pitting)
                _driverTemplates[index].SetTimingState(DriverTimeState.Pit);
            else if (driverData.LapData.pitStatus == PitStatus.In_Pit_Area)
                _driverTemplates[index].SetTimingState(DriverTimeState.Pit_Area);
            else
                return false;
            return true;
        }

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
        protected int GetTemplateIndex(DriverData driverData)
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