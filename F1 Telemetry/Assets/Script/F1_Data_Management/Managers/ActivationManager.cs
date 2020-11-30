using UnityEngine;

namespace F1_Unity
{
    public class ActivationManager : MonoBehaviour
    {
        [SerializeField] GameObject _all;
        [SerializeField] GameObject _liveSpeed;
        [SerializeField] TimingScreen _timingScreen;
        [SerializeField] GameObject _driverName;
        [SerializeField] GameObject _detailDelta;

        private void OnEnable()
        {
            InputManager.PressedToggleAll += ToggleAll;
            InputManager.PressedToggleLiveSpeed += ToggleLiveSpeed;
            InputManager.PressedToggleDriverName += ToggleDriverName;
            InputManager.PressedToggleDetailDelta += ToggleDetailDelta;
        }

        private void OnDisable()
        {
            InputManager.PressedToggleAll -= ToggleAll;
            InputManager.PressedToggleLiveSpeed -= ToggleLiveSpeed;
            InputManager.PressedToggleDriverName -= ToggleDriverName;
            InputManager.PressedToggleDetailDelta -= ToggleDetailDelta;
        }

        void ToggleAll()
        {
            bool current = !_all.activeSelf;
            _all.SetActive(current);
            _timingScreen.SetActive(current);
        }
        void ToggleLiveSpeed() { _liveSpeed.SetActive(!_liveSpeed.activeSelf); }

        void ToggleDriverName()
        {
            _driverName.SetActive(!_driverName.activeSelf);
            _detailDelta.SetActive(false);
        }

        void ToggleDetailDelta()
        {
            _detailDelta.SetActive(!_detailDelta.activeSelf);
            _driverName.SetActive(false);
        }
    }
}
