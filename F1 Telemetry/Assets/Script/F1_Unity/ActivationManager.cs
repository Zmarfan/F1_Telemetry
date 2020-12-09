using UnityEngine;

namespace F1_Unity
{
    public class ActivationManager : MonoBehaviour
    {
        [Header("Drop")]

        [SerializeField] Transform _canvas;

        [SerializeField] GameObject _all;
        [SerializeField] TimingScreen _timingScreen;
        [SerializeField] ToggleActivatable _liveSpeed;
        [SerializeField] ToggleActivatable _driverName;
        [SerializeField] ToggleActivatable _detailDelta;
        [SerializeField] ToggleActivatable _tyreWear;
        [SerializeField] ToggleActivatable _speedCompare;
        [SerializeField] ToggleActivatable _location;
        [SerializeField] ToggleActivatable _haloHud;
        [SerializeField] ToggleActivatable _lapComparision;
        [SerializeField] ToggleActivatable _ersCompare;
        [SerializeField] ToggleActivatable _circuitInfo;
        [SerializeField] ToggleActivatable _weather;
        [SerializeField] ToggleActivatablePit _pitTimer;

        [SerializeField] ToggleActivatable[] _lowerSlot;
        [SerializeField] ToggleActivatable[] _rightSlot;
        [SerializeField] ToggleActivatable[] _upperRightSlot;

        private void OnEnable()
        {
            InputManager.PressedToggleAll += ToggleAll;
            InputManager.PressedToggleLiveSpeed += ToggleLiveSpeed;
            InputManager.PressedToggleDriverName += ToggleDriverName;
            InputManager.PressedToggleDetailDelta += ToggleDetailDelta;
            InputManager.PressedToggleTyreWear += ToggleTyreWear;
            InputManager.PressedToggleSpeedCompare += ToggleSpeedCompare;
            InputManager.PressedToggleLocation += ToggleLocation;
            InputManager.PressedToggleHaloHud += ToggleHaloHud;
            InputManager.PressedToggleLapComparision += ToggleLapComparision;
            InputManager.PressedToggleERSCompare += ToggleERSCompare;
            InputManager.PressedToggleCircuitInfo += ToggleCircuitInfo;
            InputManager.PressedToggleWeather += ToggleWeather;
            InputManager.PressedTogglePitTimer += TogglePitTimer;
        }

        private void OnDisable()
        {
            InputManager.PressedToggleAll -= ToggleAll;
            InputManager.PressedToggleLiveSpeed -= ToggleLiveSpeed;
            InputManager.PressedToggleDriverName -= ToggleDriverName;
            InputManager.PressedToggleDetailDelta -= ToggleDetailDelta;
            InputManager.PressedToggleTyreWear -= ToggleTyreWear;
            InputManager.PressedToggleSpeedCompare -= ToggleSpeedCompare;
            InputManager.PressedToggleLocation -= ToggleLocation;
            InputManager.PressedToggleHaloHud -= ToggleHaloHud;
            InputManager.PressedToggleLapComparision -= ToggleLapComparision;
            InputManager.PressedToggleERSCompare -= ToggleERSCompare;
            InputManager.PressedToggleCircuitInfo -= ToggleCircuitInfo;
            InputManager.PressedToggleWeather -= ToggleWeather;
            InputManager.PressedTogglePitTimer -= TogglePitTimer;
        }

        /// <summary>
        /// Turns off all activatables in a list except for an exception
        /// </summary>
        void TurnOffActivatableArray(ToggleActivatable exception, ToggleActivatable[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] != exception)
                    array[i].Toggle(false);
            }
        }

        void ToggleAll()
        {
            bool current = !_all.activeSelf;
            _all.SetActive(current);
            _timingScreen.SetActive(current);
        }

        void ToggleDriverName()
        {
            _driverName.Toggle(!_driverName.gameObject.activeSelf);
            TurnOffActivatableArray(_driverName, _lowerSlot);
        }

        void ToggleDetailDelta()
        {
            _detailDelta.Toggle(!_detailDelta.gameObject.activeSelf);
            TurnOffActivatableArray(_detailDelta, _lowerSlot);
        }

        void ToggleSpeedCompare()
        {
            _speedCompare.Toggle(!_speedCompare.gameObject.activeSelf);
            TurnOffActivatableArray(_speedCompare, _lowerSlot);
        }

        void ToggleLapComparision()
        {
            _lapComparision.Toggle(!_lapComparision.gameObject.activeSelf);
            TurnOffActivatableArray(_lapComparision, _lowerSlot);
        }

        void ToggleERSCompare()
        {
            _ersCompare.Toggle(!_ersCompare.gameObject.activeSelf);
            TurnOffActivatableArray(_ersCompare, _lowerSlot);
        }

        void ToggleLiveSpeed()
        {
            _liveSpeed.Toggle(!_liveSpeed.gameObject.activeSelf);
            TurnOffActivatableArray(_liveSpeed, _rightSlot);
        }

        void ToggleTyreWear()
        {
            _tyreWear.Toggle(!_tyreWear.gameObject.activeSelf);
            TurnOffActivatableArray(_tyreWear, _rightSlot);
        }

        void ToggleLocation()
        {
            _location.Toggle(!_location.gameObject.activeSelf);
            TurnOffActivatableArray(_location, _upperRightSlot);
        }

        void ToggleCircuitInfo()
        {
            _circuitInfo.Toggle(!_circuitInfo.gameObject.activeSelf);
            TurnOffActivatableArray(_circuitInfo, _lowerSlot);
        }

        void ToggleWeather()
        {
            _weather.Toggle(!_weather.gameObject.activeSelf);
            TurnOffActivatableArray(_weather, _upperRightSlot);
        }

        void ToggleHaloHud()
        {
            _haloHud.Toggle(!_haloHud.gameObject.activeSelf);
        }

        void TogglePitTimer()
        {
            _pitTimer.Toggle(!_pitTimer.CurrentState);
            TurnOffActivatableArray(_pitTimer, _rightSlot);
        }
    }
}
