using UnityEngine;

namespace F1_Unity
{
    public class ActivationManager : MonoBehaviour
    {
        [Header("Drop")]

        [SerializeField] Transform _canvas;

        [SerializeField] GameObject _all;
        [SerializeField] GameObject _liveSpeed;
        [SerializeField] TimingScreen _timingScreen;
        [SerializeField] GameObject _driverName;
        [SerializeField] GameObject _detailDelta;
        [SerializeField] GameObject _tyreWear;
        [SerializeField] GameObject _speedCompare;
        [SerializeField] GameObject _location;
        [SerializeField] GameObject _haloHud;

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
        }

        void ToggleAll()
        {
            bool current = !_all.activeSelf;
            _all.SetActive(current);
            _timingScreen.SetActive(current);
        }

        void ToggleLiveSpeed()
        {
            _liveSpeed.SetActive(!_liveSpeed.activeSelf);
            _tyreWear.SetActive(false);
        }

        void ToggleDriverName()
        {
            _driverName.SetActive(!_driverName.activeSelf);
            _detailDelta.SetActive(false);
            _speedCompare.SetActive(false);
        }

        void ToggleDetailDelta()
        {
            _detailDelta.SetActive(!_detailDelta.activeSelf);
            _driverName.SetActive(false);
            _speedCompare.SetActive(false);
        }

        void ToggleTyreWear()
        {
            _tyreWear.SetActive(!_tyreWear.activeSelf);
            _liveSpeed.SetActive(false);
        }

        void ToggleSpeedCompare()
        {
            _speedCompare.SetActive(!_speedCompare.activeSelf);
            _driverName.SetActive(false);
            _detailDelta.SetActive(false);
        }

        void ToggleLocation()
        {
            _location.GetComponent<Location>().Init();
        }

        void ToggleHaloHud()
        {
            _haloHud.SetActive(!_haloHud.activeSelf);
        }
    }
}
