using UnityEngine;

namespace F1_Unity
{
    public class ActivationManager : MonoBehaviour
    {
        [Header("Drop")]

        [SerializeField] Transform _canvas;

        [SerializeField] GameObject _all;
        [SerializeField] ToggleActivatable _liveSpeed;
        [SerializeField] ToggleActivatable _driverName;
        [SerializeField] ToggleActivatable _detailDelta;
        [SerializeField] ToggleActivatable _detailDeltaLeader;
        [SerializeField] ToggleActivatable _tyreWear;
        [SerializeField] ToggleActivatable _speedCompare;
        [SerializeField] ToggleActivatable _location;
        [SerializeField] ToggleActivatable _haloHud;
        [SerializeField] ToggleActivatable _lapComparision;
        [SerializeField] ToggleActivatable _ersCompare;
        [SerializeField] ToggleActivatable _circuitInfo;
        [SerializeField] ToggleActivatable _weather;
        [SerializeField] ToggleActivatablePit _pitTimer;
        [SerializeField] ToggleActivatable _driverNameChampionship;
        [SerializeField] ToggleActivatable _qTimingUI;

        [SerializeField] ToggleActivatable[] _lowerSlot;
        [SerializeField] ToggleActivatable[] _rightSlot;
        [SerializeField] ToggleActivatable[] _upperRightSlot;

        private void OnEnable()
        {
            GameManager.InputManager.PressedToggleAll += ToggleAll;
            GameManager.InputManager.PressedToggleLiveSpeed += ToggleLiveSpeed;
            GameManager.InputManager.PressedToggleDriverName += ToggleDriverName;
            GameManager.InputManager.PressedToggleDetailDelta += ToggleDetailDelta;
            GameManager.InputManager.PressedToggleDetailDeltaLeader += ToggleDetailDeltaLeader;
            GameManager.InputManager.PressedToggleTyreWear += ToggleTyreWear;
            GameManager.InputManager.PressedToggleSpeedCompare += ToggleSpeedCompare;
            GameManager.InputManager.PressedToggleLocation += ToggleLocation;
            GameManager.InputManager.PressedToggleHaloHud += ToggleHaloHud;
            GameManager.InputManager.PressedToggleLapComparision += ToggleLapComparision;
            GameManager.InputManager.PressedToggleERSCompare += ToggleERSCompare;
            GameManager.InputManager.PressedToggleCircuitInfo += ToggleCircuitInfo;
            GameManager.InputManager.PressedToggleWeather += ToggleWeather;
            GameManager.InputManager.PressedTogglePitTimer += TogglePitTimer;
            GameManager.InputManager.PressedToggleDriverNameChampionship += ToggleDriverNameChampionship;
            GameManager.InputManager.PressedQTimingUI += ToggleQTimingUI;
        }

        private void OnDisable()
        {
            GameManager.InputManager.PressedToggleAll -= ToggleAll;
            GameManager.InputManager.PressedToggleLiveSpeed -= ToggleLiveSpeed;
            GameManager.InputManager.PressedToggleDriverName -= ToggleDriverName;
            GameManager.InputManager.PressedToggleDetailDelta -= ToggleDetailDelta;
            GameManager.InputManager.PressedToggleDetailDeltaLeader -= ToggleDetailDeltaLeader;
            GameManager.InputManager.PressedToggleTyreWear -= ToggleTyreWear;
            GameManager.InputManager.PressedToggleSpeedCompare -= ToggleSpeedCompare;
            GameManager.InputManager.PressedToggleLocation -= ToggleLocation;
            GameManager.InputManager.PressedToggleHaloHud -= ToggleHaloHud;
            GameManager.InputManager.PressedToggleLapComparision -= ToggleLapComparision;
            GameManager.InputManager.PressedToggleERSCompare -= ToggleERSCompare;
            GameManager.InputManager.PressedToggleCircuitInfo -= ToggleCircuitInfo;
            GameManager.InputManager.PressedToggleWeather -= ToggleWeather;
            GameManager.InputManager.PressedTogglePitTimer -= TogglePitTimer;
            GameManager.InputManager.PressedToggleDriverNameChampionship -= ToggleDriverNameChampionship;
            GameManager.InputManager.PressedQTimingUI -= ToggleQTimingUI;
        }

        /// <summary>
        /// Clear out all lingering data each activatable holds
        /// </summary>
        public void ClearData()
        {
            ClearList(_lowerSlot);
            ClearList(_rightSlot);
            ClearList(_upperRightSlot);
        }

        /// <summary>
        /// Clears data from a specific range of activatables
        /// </summary>
        /// <param name="list"></param>
        void ClearList(ToggleActivatable[] list)
        {
            for (int i = 0; i < list.Length; i++)
            {
                IActivatableReset reset = list[i].gameObject.GetComponent<IActivatableReset>();
                if (reset != null)
                    reset.ClearActivatable();
            }
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
            GameManager.TimingScreenManager.SetActive(current);
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

        void ToggleDetailDeltaLeader()
        {
            _detailDeltaLeader.Toggle(!_detailDeltaLeader.gameObject.activeSelf);
            TurnOffActivatableArray(_detailDeltaLeader, _lowerSlot);
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

        void ToggleDriverNameChampionship()
        {
            _driverNameChampionship.Toggle(!_driverNameChampionship.gameObject.activeSelf);
            TurnOffActivatableArray(_driverNameChampionship, _lowerSlot);
        }

        void ToggleQTimingUI()
        {
            _qTimingUI.Toggle(!_qTimingUI.gameObject.activeSelf);
            TurnOffActivatableArray(_qTimingUI, _rightSlot);
        }
    }
}
