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

        private void Awake()
        {
            if (_singleton == null)
                _singleton = this;
            else
                Destroy(this.gameObject);
        }

        private void Update()
        {
            if (Input.GetButtonDown(InputKeywords.TIME_INTERVAL))
                PressedTimeInterval?.Invoke();
            if (Input.GetButtonDown(InputKeywords.LIVE_SPEED))
                PressedToggleLiveSpeed?.Invoke();
            if (Input.GetButtonDown(InputKeywords.TOGGLE_ALL))
                PressedToggleAll?.Invoke();
        }

    }
}