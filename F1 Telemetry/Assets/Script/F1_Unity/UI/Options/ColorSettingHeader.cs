using UnityEngine.UI;
using UnityEngine;

namespace F1_Options
{
    public class ColorSettingHeader : MonoBehaviour
    {
        [SerializeField] Text _headerText;

        /// <summary>
        /// Sets the text of the header
        /// </summary>
        public void SetHeaderText(string text)
        {
            _headerText.text = text;
        }
    }
}