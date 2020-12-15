using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace F1_Unity
{
    public delegate void InputPressedDown(); 
    public delegate void InputFloat(float value); 

    /// <summary>
    /// Main source of getting inputs
    /// </summary>
    public class InputManager : MonoBehaviour
    {
        static InputManager _singleton;

        public static event InputPressedDown PressedQuitGame;

        public static event InputPressedDown PressedTimeInterval;
        public static event InputPressedDown PressedToggleLiveSpeed;
        public static event InputPressedDown PressedToggleAll;
        public static event InputPressedDown PressedToggleDriverName;
        public static event InputPressedDown PressedToggleDetailDelta;
        public static event InputPressedDown PressedToggleTyreWear;
        public static event InputPressedDown PressedToggleSpeedCompare;
        public static event InputPressedDown PressedToggleLocation;
        public static event InputPressedDown PressedToggleHaloHud;
        public static event InputPressedDown PressedToggleLapComparision;
        public static event InputPressedDown PressedToggleERSCompare;
        public static event InputPressedDown PressedToggleCircuitInfo;
        public static event InputPressedDown PressedToggleWeather;
        public static event InputPressedDown PressedTogglePitTimer;
        public static event InputPressedDown PressedToggleDriverNameChampionship;

        private void Awake()
        {
            if (_singleton == null)
                _singleton = this;
            else
                Destroy(this.gameObject);
        }

        private void Update()
        {
            if (Input.GetButton(InputKeywords.TIMING))       //TIMING => t
                CheckTiming();
            else if (Input.GetButton(InputKeywords.INFO))    //INFO => i
                CheckInformation();        
            else if (Input.GetButton(InputKeywords.COMPARE)) //COMPARE => u
                CheckCompare();        
            else if (Input.GetButton(InputKeywords.DRIVER))  //DRIVER => y
                CheckDriver();
            else                                             //MISC -> NONE
                CheckMisc();
        }

        /// <summary>
        /// Invokes input events involving timing 
        /// </summary>
        void CheckTiming()
        {
            if (Input.GetButtonDown(InputKeywords.TIME_INTERVAL))   //1
                PressedTimeInterval?.Invoke();
        }

        /// <summary>
        /// Invokes input events involving comparision of drivers
        /// </summary>
        void CheckCompare()
        {
            if (Input.GetButtonDown(InputKeywords.DRIVER_NAME))          //1
                PressedToggleDriverName?.Invoke();
            else if (Input.GetButtonDown(InputKeywords.DETAIL_DELTA))    //2
                PressedToggleDetailDelta?.Invoke();
            else if (Input.GetButtonDown(InputKeywords.LAP_COMPARISION)) //3
                PressedToggleLapComparision?.Invoke();
            else if (Input.GetButtonDown(InputKeywords.SPEED_COMPARE))   //4
                PressedToggleSpeedCompare?.Invoke();
            else if (Input.GetButtonDown(InputKeywords.ERS_COMPARE))     //5
                PressedToggleERSCompare?.Invoke();
        }

        /// <summary>
        /// Invokes input events involving driver detail and information
        /// </summary>
        void CheckDriver()
        {
            if (Input.GetButtonDown(InputKeywords.LIVE_SPEED))                     //1
                PressedToggleLiveSpeed?.Invoke();                                  
            else if (Input.GetButtonDown(InputKeywords.TYRE_WEAR))                 //2
                PressedToggleTyreWear?.Invoke();                                   
            else if (Input.GetButtonDown(InputKeywords.PIT_TIMER))                 //3
                PressedTogglePitTimer?.Invoke();
            else if (Input.GetButtonDown(InputKeywords.DRIVER_NAME_CHAMPIONSHIP))  //4
                PressedToggleDriverNameChampionship?.Invoke();
        }

        /// <summary>
        /// Invokes information input events
        /// </summary>
        void CheckInformation()
        {
            if (Input.GetButtonDown(InputKeywords.LOCATION))             //1
                PressedToggleLocation?.Invoke();
            else if (Input.GetButtonDown(InputKeywords.CIRCUIT_INFO))    //2
                PressedToggleCircuitInfo?.Invoke();
            else if (Input.GetButtonDown(InputKeywords.WEATHER))         //3
                PressedToggleWeather?.Invoke();
        }

        /// <summary>
        /// Invokes misc input events
        /// </summary>
        void CheckMisc()
        {
            if (Input.GetButtonDown(InputKeywords.TOGGLE_ALL))  //M
                PressedToggleAll?.Invoke();
            if (Input.GetButtonDown(InputKeywords.HALO_HUD))    //H
                PressedToggleHaloHud?.Invoke();
        }
    }
}