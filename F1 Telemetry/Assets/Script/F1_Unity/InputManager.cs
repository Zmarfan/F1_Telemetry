using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RawInput;

namespace F1_Unity
{
    public delegate void InputPressedDown(); 
    public delegate void InputFloat(float value); 

    /// <summary>
    /// Main source of getting inputs
    /// </summary>
    public class InputManager : MonoBehaviour
    {
        #region Input Settings

        [Header("Input Keys")]

        [SerializeField] Key _toggleAllKey = Key.M;
        [SerializeField] Key _lowerKey = Key.Z;
        [SerializeField] Key _rightKey = Key.X;
        [SerializeField] Key _upperRightKey = Key.C;
        [SerializeField] Key _timingKey = Key.V;
        [SerializeField] Key _haloHudKey = Key.H;

        [Header("Lower")]

        [SerializeField] Key _driverNameKey = Key.Q;
        [SerializeField] Key _detailDeltaKey = Key.W;
        [SerializeField] Key _detailDeltaLeaderKey = Key.E;
        [SerializeField] Key _speedCompareKey = Key.E;
        [SerializeField] Key _lapComparisionKey = Key.R;
        [SerializeField] Key _ersCompareKey = Key.T;
        [SerializeField] Key _circuitInfoKey = Key.Y;
        [SerializeField] Key _driverNameChampionshipKey = Key.U;

        [Header("Right")]

        [SerializeField] Key _liveSpeedKey = Key.Q;
        [SerializeField] Key _tyreWearKey = Key.W;
        [SerializeField] Key _pitTimerKey = Key.E;
        [SerializeField] Key _qTimingUI = Key.R;

        [Header("Upper Right")]

        [SerializeField] Key _locationKey = Key.Q;
        [SerializeField] Key _weatherKey = Key.W;

        [Header("Timing")]

        [SerializeField] Key _timingIntervalTypeKey = Key.Q;
        [SerializeField] Key _timingStatsStateKey = Key.W;
        [SerializeField] Key _timingNameMode = Key.E;

        #endregion

        #region Events

        //Misc
        public event InputPressedDown PressedToggleAll;
        public event InputPressedDown PressedToggleHaloHud;
        //Timing 
        public event InputPressedDown PressedTimeInterval;
        public event InputPressedDown PressedTimeStatsState;
        public event InputPressedDown PressedTimingNameMode;
        //Lower
        public event InputPressedDown PressedToggleDriverName;
        public event InputPressedDown PressedToggleDetailDelta;
        public event InputPressedDown PressedToggleDetailDeltaLeader;
        public event InputPressedDown PressedToggleSpeedCompare;
        public event InputPressedDown PressedToggleLapComparision;
        public event InputPressedDown PressedToggleERSCompare;
        public event InputPressedDown PressedToggleCircuitInfo;
        public event InputPressedDown PressedToggleDriverNameChampionship;
        //Right
        public event InputPressedDown PressedToggleLiveSpeed;
        public event InputPressedDown PressedToggleTyreWear;
        public event InputPressedDown PressedTogglePitTimer;
        public event InputPressedDown PressedQTimingUI;
        //Upper Right
        public event InputPressedDown PressedToggleLocation;
        public event InputPressedDown PressedToggleWeather;

        #endregion

        #region Enable Disable

        void OnEnable()
        {
            GameManager.RawInputSystem.SubscribeToKeyEventDown(_toggleAllKey, ToggleAll);
            GameManager.RawInputSystem.SubscribeToKeyEventDown(_haloHudKey, ToggleHaloHud);

            GameManager.RawInputSystem.SubscribeToKeyEventDown(_timingIntervalTypeKey, CheckTiming);
            GameManager.RawInputSystem.SubscribeToKeyEventDown(_timingStatsStateKey, CheckTiming);
            GameManager.RawInputSystem.SubscribeToKeyEventDown(_timingNameMode, CheckTiming);

            GameManager.RawInputSystem.SubscribeToKeyEventDown(_driverNameKey, CheckLower);
            GameManager.RawInputSystem.SubscribeToKeyEventDown(_detailDeltaKey, CheckLower);
            GameManager.RawInputSystem.SubscribeToKeyEventDown(_detailDeltaLeaderKey, CheckLower);
            GameManager.RawInputSystem.SubscribeToKeyEventDown(_speedCompareKey, CheckLower);
            GameManager.RawInputSystem.SubscribeToKeyEventDown(_lapComparisionKey, CheckLower);
            GameManager.RawInputSystem.SubscribeToKeyEventDown(_ersCompareKey, CheckLower);
            GameManager.RawInputSystem.SubscribeToKeyEventDown(_circuitInfoKey, CheckLower);
            GameManager.RawInputSystem.SubscribeToKeyEventDown(_driverNameChampionshipKey, CheckLower);

            GameManager.RawInputSystem.SubscribeToKeyEventDown(_liveSpeedKey, CheckRight);
            GameManager.RawInputSystem.SubscribeToKeyEventDown(_tyreWearKey, CheckRight);
            GameManager.RawInputSystem.SubscribeToKeyEventDown(_pitTimerKey, CheckRight);
            GameManager.RawInputSystem.SubscribeToKeyEventDown(_qTimingUI, CheckRight);

            GameManager.RawInputSystem.SubscribeToKeyEventDown(_locationKey, CheckUpperRight);
            GameManager.RawInputSystem.SubscribeToKeyEventDown(_weatherKey, CheckUpperRight);
        }

        void OnDisable()
        {
            GameManager.RawInputSystem.UnsubscribeToKeyEventDown(_toggleAllKey, ToggleAll);
            GameManager.RawInputSystem.UnsubscribeToKeyEventDown(_haloHudKey, ToggleHaloHud);

            GameManager.RawInputSystem.UnsubscribeToKeyEventDown(_timingIntervalTypeKey, CheckTiming);
            GameManager.RawInputSystem.UnsubscribeToKeyEventDown(_timingStatsStateKey, CheckTiming);
            GameManager.RawInputSystem.UnsubscribeToKeyEventDown(_timingNameMode, CheckTiming);

            GameManager.RawInputSystem.UnsubscribeToKeyEventDown(_driverNameKey, CheckLower);
            GameManager.RawInputSystem.UnsubscribeToKeyEventDown(_detailDeltaKey, CheckLower);
            GameManager.RawInputSystem.UnsubscribeToKeyEventDown(_detailDeltaLeaderKey, CheckLower);
            GameManager.RawInputSystem.UnsubscribeToKeyEventDown(_speedCompareKey, CheckLower);
            GameManager.RawInputSystem.UnsubscribeToKeyEventDown(_lapComparisionKey, CheckLower);
            GameManager.RawInputSystem.UnsubscribeToKeyEventDown(_ersCompareKey, CheckLower);
            GameManager.RawInputSystem.UnsubscribeToKeyEventDown(_circuitInfoKey, CheckLower);
            GameManager.RawInputSystem.UnsubscribeToKeyEventDown(_driverNameChampionshipKey, CheckLower);

            GameManager.RawInputSystem.UnsubscribeToKeyEventDown(_liveSpeedKey, CheckRight);
            GameManager.RawInputSystem.UnsubscribeToKeyEventDown(_tyreWearKey, CheckRight);
            GameManager.RawInputSystem.UnsubscribeToKeyEventDown(_pitTimerKey, CheckRight);
            GameManager.RawInputSystem.UnsubscribeToKeyEventDown(_qTimingUI, CheckRight);

            GameManager.RawInputSystem.UnsubscribeToKeyEventDown(_locationKey, CheckUpperRight);
            GameManager.RawInputSystem.UnsubscribeToKeyEventDown(_weatherKey, CheckUpperRight);
        }

        #endregion

        #region Callback Methods

        /// <summary>
        /// Sends out input for toggle all
        /// </summary>
        void ToggleAll(Key key)
        {
            PressedToggleAll?.Invoke();
        }

        /// <summary>
        /// Sends out input for HaloHud
        /// </summary>
        void ToggleHaloHud(Key key)
        {
            PressedToggleHaloHud?.Invoke();
        }

        /// <summary>
        /// Gets called when lower input event is called, lower section of inputs
        /// </summary>
        /// <param name="key">Key that was pressed for lower group</param>
        void CheckLower(Key key)
        {
            //Is group key held down?
            if (GameManager.RawInputSystem.IsKeyDown(_lowerKey))
            {
                if (key == _driverNameKey)                      { PressedToggleDriverName?.Invoke(); }
                else if (key == _detailDeltaKey)                { PressedToggleDetailDelta?.Invoke(); }
                else if (key == _detailDeltaLeaderKey)          { PressedToggleDetailDeltaLeader?.Invoke(); }
                else if (key == _speedCompareKey)               { PressedToggleSpeedCompare?.Invoke(); }
                else if (key == _lapComparisionKey)             { PressedToggleLapComparision?.Invoke(); }
                else if (key == _ersCompareKey)                 { PressedToggleERSCompare?.Invoke(); }
                else if (key == _circuitInfoKey)                { PressedToggleCircuitInfo?.Invoke(); }
                else if (key == _driverNameChampionshipKey)     { PressedToggleDriverNameChampionship?.Invoke(); }
                else { throw new System.Exception("There exist no handling for this input key: " + key); }
            }
        }

        /// <summary>
        /// Gets called when lower input event is called, Right section of inputs
        /// </summary>
        /// <param name="key">Key that was pressed for lower group</param>
        void CheckRight(Key key)
        {
            //Is group key held down?
            if (GameManager.RawInputSystem.IsKeyDown(_rightKey))
            {
                if (key == _liveSpeedKey)     { PressedToggleLiveSpeed?.Invoke(); }
                else if (key == _tyreWearKey) { PressedToggleTyreWear?.Invoke(); }
                else if (key == _pitTimerKey) { PressedTogglePitTimer?.Invoke(); }
                else if (key == _qTimingUI)   { PressedQTimingUI?.Invoke(); }
                else { throw new System.Exception("There exist no handling for this input key: " + key); }
            }
        }

        /// <summary>
        /// Gets called when lower input event is called, Upper Right section of inputs
        /// </summary>
        /// <param name="key">Key that was pressed for lower group</param>
        void CheckUpperRight(Key key)
        {
            //Is group key held down?
            if (GameManager.RawInputSystem.IsKeyDown(_upperRightKey))
            {
                if (key == _locationKey)     { PressedToggleLocation?.Invoke(); }
                else if (key == _weatherKey) { PressedToggleWeather?.Invoke(); }
                else { throw new System.Exception("There exist no handling for this input key: " + key); }
             }
        }

        /// <summary>
        /// Gets called when lower input event is called, timing section of inputs
        /// </summary>
        /// <param name="key">Key that was pressed for lower group</param>
        void CheckTiming(Key key)
        {
            //Is group key held down?
            if (GameManager.RawInputSystem.IsKeyDown(_timingKey))
            {
                if (key == _timingIntervalTypeKey)    { PressedTimeInterval?.Invoke(); }
                else if (key == _timingStatsStateKey) { PressedTimeStatsState?.Invoke(); }
                else if (key == _timingNameMode)      { PressedTimingNameMode?.Invoke(); }
                else { throw new System.Exception("There exist no handling for this input key: " + key); }
            }
        }
    }

    #endregion
}