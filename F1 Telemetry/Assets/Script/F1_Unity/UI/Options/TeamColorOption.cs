using UnityEngine.UI;
using UnityEngine;
using F1_Data_Management;
using F1_Unity;

namespace F1_Options
{
    /// <summary>
    /// User entrance to change Color for specific team
    /// </summary>
    public class TeamColorOption : MonoBehaviour
    {
        public delegate void TeamColorChangeDelegate(TeamColorPair teamColorPair);

        [Header("Settings")]

        [SerializeField] GameObject _colorPickerPrefab;

        [Header("Drop")]

        [SerializeField] Text _teamText;
        [SerializeField] Image _teamColorImage;

        //The color picker for this team option
        ColorPicker _colorPicker;

        /// <summary>
        /// Invoked when a new color is confirmed for new team by user
        /// </summary>
        public event TeamColorChangeDelegate NewColor;

        TeamColorPair _teamColorPair;

        /// <summary>
        /// Initilizes the prefab with specific team and color
        /// </summary>
        public void Init(TeamColorPair teamColorPair)
        {
            _teamColorPair = teamColorPair;

            _teamText.text = ConvertEnumToString.Convert<Team>(_teamColorPair.team);
            _teamColorPair.currentColor = teamColorPair.currentColor;
        }

        /// <summary>
        /// Called when user presses button to change color for this specific team
        /// </summary>
        public void SpawnColorPicker()
        {
            GameObject obj = Instantiate(_colorPickerPrefab, GetComponentInParent<Canvas>().transform);
            _colorPicker = obj.GetComponent<ColorPicker>();
            _colorPicker.Init(_teamColorPair.currentColor);
            _colorPicker.ChangedColor += ChangeColor;
            _colorPicker.PickedColor += PickedColor;
        }

        /// <summary>
        /// Called when user wants default color -> set the default color as given from Init
        /// </summary>
        public void SetDefaultColor()
        {
            SetColor(_teamColorPair.DefaultColor);
        }

        /// <summary>
        /// Sets the current color of this team to memory and interface
        /// </summary>
        void SetColor(Color color)
        {
            _teamColorImage.color = color;
            _teamColorPair.currentColor = color;
        }

        #region Listeners

        /// <summary>
        /// Called when user is changing color in ColorPicker
        /// </summary>
        /// <param name="color">Currently viewed color</param>
        void ChangeColor(Color color)
        {
            _teamColorImage.color = color;
        }

        /// <summary>
        /// Called when user confirms new or stay with old color
        /// </summary>
        /// <param name="color">Either new or old color</param>
        void PickedColor(Color color)
        {
            SetColor(color);

            //Remove colorpicker listeners and remove it
            _colorPicker.ChangedColor -= ChangeColor;
            _colorPicker.PickedColor -= PickedColor;
            Destroy(_colorPicker.gameObject);
        }

        #endregion
    }
}