using UnityEngine;

namespace F1_Unity
{
    public class ToggleActivatablePit : ToggleActivatable
    {
        [SerializeField] CanvasGroup _canvasGroup;

        public bool CurrentState { get; private set; }

        private void Awake()
        {
            Toggle(false);
        }

        public override void Toggle(bool status)
        {
            CurrentState = status;
            _canvasGroup.alpha = status ? 1.0f : 0.0f;
        }
    }
}