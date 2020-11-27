using UnityEngine;

namespace F1_Unity
{
    public class ActivationManager : MonoBehaviour
    {
        [SerializeField] GameObject _all;
        [SerializeField] GameObject _liveSpeed;

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

        void ToggleAll() { _all.SetActive(!_all.activeSelf); }
        void ToggleLiveSpeed() { _liveSpeed.SetActive(!_liveSpeed.activeSelf); }
    }
}
