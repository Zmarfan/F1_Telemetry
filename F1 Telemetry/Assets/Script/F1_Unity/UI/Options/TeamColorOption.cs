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
        [Header("Settings")]

        [SerializeField] GameObject _colorPickerPrefab;
        [SerializeField, Range(0, 1)] float _dullBackgroundColorAmount = 0.5f;

        [Header("Drop")]

        [SerializeField] Text _teamText;
        [SerializeField] Image _teamColorImage;
        [SerializeField] Image _background;

        //The color picker for this team option
        ColorPicker _colorPicker;

        TeamColorData _teamColorPair;

        /// <summary>
        /// Initilizes the prefab with specific team and color
        /// </summary>
        public void Init(TeamColorData teamColorPair)
        {
            _teamColorPair = teamColorPair;

            _teamText.text = ConvertEnumToString.Convert<Team>(_teamColorPair.team);
            _teamColorPair.currentColor = teamColorPair.currentColor;

            SetColor(_teamColorPair.currentColor, true);
        }

        /// <summary>
        /// Returns this options stored team color data
        /// </summary>
        public TeamColorData TeamColor { get { return _teamColorPair; } }

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
            SetColor(_teamColorPair.defaultColor, true);
        }

        /// <summary>
        /// Sets the current color of this team to memory and interface
        /// </summary>
        void SetColor(Color color, bool save)
        {
            _teamColorImage.color = color;
            _background.color = DullColor(color);
            if (save)
                _teamColorPair.currentColor = color;
        }

        /// <summary>
        /// Returns color but with lowered HSL saturation mode
        /// </summary>
        Color DullColor(Color color)
        {
            HSLColor hslColor = HSLColor.HSLFromRGB(color);
            hslColor.s *= _dullBackgroundColorAmount;
            return hslColor.RGBColor();
        }

        #region Listeners

        /// <summary>
        /// Called when user is changing color in ColorPicker
        /// </summary>
        /// <param name="color">Currently viewed color</param>
        void ChangeColor(Color color)
        {
            SetColor(color, false);
        }

        /// <summary>
        /// Called when user confirms new or stay with old color
        /// </summary>
        /// <param name="color">Either new or old color</param>
        void PickedColor(Color color)
        {
            SetColor(color, true);

            //Remove colorpicker listeners and remove it
            _colorPicker.ChangedColor -= ChangeColor;
            _colorPicker.PickedColor -= PickedColor;
            Destroy(_colorPicker.gameObject);
        }

        #endregion
    }
}