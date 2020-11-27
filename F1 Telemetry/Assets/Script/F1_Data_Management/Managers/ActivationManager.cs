using UnityEngine;

namespace F1_Unity
{
    public class ActivationManager : MonoBehaviour
    {
        [SerializeField] GameObject _all;
        [SerializeField] GameObject _liveSpeed;
        [SerializeField] TimingScreen _timingScreen;

        private void OnEnable()
        {
            InputManager.PressedToggleAll += ToggleAll;
            InputManager.PressedToggleLiveSpeed += ToggleLiveSpeed;
        }

        private void OnDisable()
        {
            InputManager.PressedToggleAll -= ToggleAll;
            InputManager.PressedToggleLiveSpeed -= ToggleLiveSpeed;
        }

        void ToggleAll()
        {
            bool current = !_all.activeSelf;
            _all.SetActive(current);
            _timingScreen.SetActive(current);
        }
        void ToggleLiveSpeed() { _liveSpeed.SetActive(!_liveSpeed.activeSelf); }
    }
}
