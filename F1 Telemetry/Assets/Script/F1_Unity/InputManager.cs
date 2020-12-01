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

        public static event InputPressedDown PressedTimeInterval;
        public static event InputPressedDown PressedToggleLiveSpeed;
        public static event InputPressedDown PressedToggleAll;
        public static event InputPressedDown PressedToggleDriverName;
        public static event InputPressedDown PressedToggleDetailDelta;
        public static event InputPressedDown PressedToggleTyreWear;

        private void Awake()
        {
            if (_singleton == null)
                _singleton = this;
            else
                Destroy(this.gameObject);
        }

        private void Update()
        {
            if (Input.GetButtonDown(InputKeywords.DRIVER_NAME))   //R
                PressedToggleDriverName?.Invoke();
            if (Input.GetButtonDown(InputKeywords.TIME_INTERVAL)) //T
                PressedTimeInterval?.Invoke();
            if (Input.GetButtonDown(InputKeywords.DETAIL_DELTA))  //Y
                PressedToggleDetailDelta?.Invoke();
            if (Input.GetButtonDown(InputKeywords.LIVE_SPEED))    //L
                PressedToggleLiveSpeed?.Invoke();
            if (Input.GetButtonDown(InputKeywords.TYRE_WEAR))     //K
                PressedToggleTyreWear?.Invoke();
            if (Input.GetButtonDown(InputKeywords.TOGGLE_ALL))    //M
                PressedToggleAll?.Invoke();
        }

    }
}