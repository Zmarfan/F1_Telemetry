using UnityEngine;

namespace F1_Unity
{
    public class ToggleActivatable : MonoBehaviour
    {
        public virtual void Toggle(bool status)
        {
            gameObject.SetActive(status);
        }
    }
}