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

        Team _team;
        Color _teamColor;

        /// <summary>
        /// Initilizes the prefab with specific team and color
        /// </summary>
        public void Init(TeamColorPair teamColorPair)
        {
            _team = teamColorPair.team;
            _teamText.text = ConvertEnumToString.Convert<Team>(_team);
            _teamColor = teamColorPair.color;
            _teamColorImage.color = _teamColor;
        }

        /// <summary>
        /// Called when user presses button to change color for this specific team
        /// </summary>
        public void SpawnColorPicker()
        {
            GameObject obj = Instantiate(_colorPickerPrefab, GetComponentInParent<Canvas>().transform);
            _colorPicker = obj.GetComponent<ColorPicker>();
            _colorPicker.Init(_teamColor);
            _colorPicker.ChangedColor += ChangeColor;
            _colorPicker.PickedColor += PickedColor;
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
            _teamColorImage.color = color;
            _teamColor = color;

            //Remove colorpicker listeners and remove it
            _colorPicker.ChangedColor -= ChangeColor;
            _colorPicker.PickedColor -= PickedColor;
            Destroy(_colorPicker.gameObject);
        }

        #endregion
    }
}