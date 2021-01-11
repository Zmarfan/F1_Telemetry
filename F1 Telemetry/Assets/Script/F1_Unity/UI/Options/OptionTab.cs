using UnityEngine.UI;
using UnityEngine;

namespace F1_Options
{
    /// <summary>
    /// Operates a specific tab in options
    /// </summary>
    public class OptionTab : MonoBehaviour
    {
        [SerializeField] Image _darkenImage;

        /// <summary>
        /// Change apperance of option tab button
        /// </summary>
        /// <param name="darkenColor">Overlay color on button</param>
        public void SetDarkenColor(Color darkenColor)
        {
            _darkenImage.color = darkenColor;
        }
    }
}