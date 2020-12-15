using UnityEngine;
using F1_Data_Management;
using UnityEngine.UI;

namespace F1_Unity
{
    public class TimingStats : MonoBehaviour
    {
        [Header("Settings")]

        [SerializeField] string _tyreLapUnavailableDataString = "?";
        [SerializeField] string _tyreLapEndingString = " Laps";
        [SerializeField] string _stopEndingString = " Stop";
        [SerializeField] string _penaltyEndingString = "s";
        [SerializeField] Sprite _positionChangedUp;
        [SerializeField] Sprite _positionChangedDown;
        [SerializeField] Sprite _positionChangedUnchanged;
        [SerializeField] Color _positionChangedUpColor;
        [SerializeField] Color _positionChangedDownColor;
        [SerializeField] Color _positionChangedUnchangedolor;

        [Header("Drop")]

        [SerializeField] Image _positionChangedImage;
        [SerializeField] Image _tyreImage;
        [SerializeField] Image _penaltyImage;
        [SerializeField] Text _positionChangedText;
        [SerializeField] Text _tyreLapText;
        [SerializeField] Text _stopText;
        [SerializeField] Text _penaltyText;

        /// <summary>
        /// Called from parent to update all the variables
        /// </summary>
        public void UpdateValues(DriverData driverData)
        {
            UpdatePositionChange(driverData);
            UpdateTyre(driverData);
            UpdateStop(driverData);
            UpdatePenalty(driverData);
        }

        #region Update Functions

        /// <summary>
        /// Updates position change image sprite, color and value for display depending on drivers current position compared to starting position
        /// </summary>
        void UpdatePositionChange(DriverData driverData)
        {
            int positionChanged = driverData.LapData.gridPosition - driverData.LapData.carPosition;
            //Gained places
            if (positionChanged > 0)
                ChangePositionSprite(_positionChangedUp, _positionChangedUpColor, positionChanged);
            //Dropped places
            else if (positionChanged < 0)
                ChangePositionSprite(_positionChangedDown, _positionChangedDownColor, positionChanged);
            //Unchanged
            else
                ChangePositionSprite(_positionChangedUnchanged, _positionChangedUnchangedolor, positionChanged);
        }

        /// <summary>
        /// Updates tyre type and laps old to display based on driver data
        /// </summary>
        void UpdateTyre(DriverData driverData)
        {
            _tyreImage.sprite = GameManager.ParticipantManager.GetVisualTyreCompoundSprite(driverData.StatusData.visualTyreCompound);
            if (driverData.ParticipantData.publicTelemetry)
                _tyreLapText.text = driverData.StatusData.tyreAgeInLaps.ToString() + _tyreLapEndingString;
            else
                _tyreLapText.text = _tyreLapUnavailableDataString + _tyreLapEndingString;
        }

        /// <summary>
        /// Updates text for amount of stops driver has currently done in the race
        /// </summary>
        void UpdateStop(DriverData driverData)
        {
            byte timesPitted = GameManager.LapManager.TimesPitted(driverData.VehicleIndex);
            _stopText.text = timesPitted + _stopEndingString;
        }

        /// <summary>
        /// Updates sprite and text for penalty situation for driver
        /// </summary>
        void UpdatePenalty(DriverData driverData)
        {
            _penaltyImage.gameObject.SetActive(driverData.LapData.totalPenalties != 0);
            _penaltyText.text = driverData.LapData.totalPenalties.ToString() + _penaltyEndingString;
        }

        /// <summary>
        /// Sets the attributes of position change
        /// </summary>
        void ChangePositionSprite(Sprite sprite, Color color, int positionChange)
        {
            _positionChangedImage.sprite = sprite;
            _positionChangedImage.color = color;
            _positionChangedText.color = color;
            _positionChangedText.text = Mathf.Abs(positionChange).ToString();
        }

        #endregion

        #region Structs

        /// <summary>
        /// What sort of information should be displayed -> penalty symbol will always show
        /// </summary>
        public enum TimingStateState
        {
            Position_Changed,
            Tyre,
            Stop,
            Penalty,
        }

        #endregion
    }
}