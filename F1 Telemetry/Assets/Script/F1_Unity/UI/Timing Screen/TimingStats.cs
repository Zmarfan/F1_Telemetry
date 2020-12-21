using UnityEngine;
using F1_Data_Management;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

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

        [SerializeField] RectTransform _holderRectTransform;
        [SerializeField] HorizontalLayoutGroup _layoutGroup;
        [SerializeField] ContentSizeFitter _holderContentSizeFitter;
        [SerializeField] Image _positionChangedImage;
        [SerializeField] Image _tyreImage;
        [SerializeField] Image _penaltyImage;
        [SerializeField] Text _positionChangedText;
        [SerializeField] Text _tyreLapText;
        [SerializeField] Text _stopText;
        [SerializeField] Text _penaltyText;

        [Header("GameObjects")]

        [SerializeField] List<GameObject> _allStatsObjects;

        [SerializeField] GameObject _positionChangedImageObj;
        [SerializeField] GameObject _positionChangedTextObj;
        [SerializeField] GameObject _startTyreObj;
        [SerializeField] GameObject _tyreLapTextObj;
        [SerializeField] GameObject _stopsObj;
        [SerializeField] GameObject _penaltyAmountObj;
        [SerializeField] GameObject _penaltyObj;

        //Used to keep track of when an update is needed -> no need to poll
        int _lastPositionChanged = int.MinValue;
        VisualTyreCompound _lastTyreCompound;
        int _lastAmountOfPenalties = int.MinValue;
        int _lastTyreLife = int.MinValue;
        int _lastAmountOfStops = int.MinValue;

        /// <summary>
        /// Changes timing state to show specific information
        /// </summary>
        /// <param name="state">What state to go to</param>
        public void ChangeState(TimingStatsState state)
        {
            switch (state)
            {
                case TimingStatsState.None:             { SetObjectState(new List<GameObject>());                                                       break; }
                case TimingStatsState.Position_Changed: { SetObjectState(new List<GameObject>() { _positionChangedImageObj, _positionChangedTextObj }); break; }
                case TimingStatsState.Tyre:             { SetObjectState(new List<GameObject>() { _startTyreObj, _tyreLapTextObj });                    break; }
                case TimingStatsState.Stop:             { SetObjectState(new List<GameObject>() { _stopsObj });                                         break; }
                case TimingStatsState.Penalty:          { SetObjectState(new List<GameObject>() { _penaltyAmountObj });                                 break; }
                case TimingStatsState.All:              { SetObjectState(_allStatsObjects);                                                             break; }
                default: { throw new System.Exception("There is no implementation for TimingStateState: " + state); }                                   
            }

            UpdateLayoutGroup();
        }

        /// <summary>
        /// Sets the activeObjects active and rest to inactive
        /// </summary>
        /// <param name="activeObjects">List of GameObjects to be activated</param>
        void SetObjectState(List<GameObject> activeObjects)
        {
            for (int i = 0; i < _allStatsObjects.Count; i++)
            {
                //All but penalty
                if (_allStatsObjects[i] != _penaltyObj)
                {
                    //Activate activeObjects and set all else inactive
                    if (activeObjects.Any(item => item == _allStatsObjects[i]))
                        _allStatsObjects[i].SetActive(true);
                    //Disable 
                    else
                        _allStatsObjects[i].SetActive(false);
                }
            }
        }

        /// <summary>
        /// Called from parent to update all the variables
        /// </summary>
        public void UpdateValues(DriverData driverData)
        {
            bool changed = false;
            UpdatePositionChange(driverData, ref changed);
            UpdateTyre(driverData, ref changed);
            UpdateStop(driverData, ref changed);
            UpdatePenalty(driverData, ref changed);

            //Align everything correctly if changes have been made
            if (changed)
                UpdateLayoutGroup();
        }

        #region Update Functions

        /// <summary>
        /// Updates so all children in horizontal layout group are padded correctly
        /// </summary>
        void UpdateLayoutGroup()
        {
            _holderContentSizeFitter.enabled = false;
            _layoutGroup.SetLayoutHorizontal();
            LayoutRebuilder.ForceRebuildLayoutImmediate(_holderRectTransform);
            _holderContentSizeFitter.enabled = true;
        }

        /// <summary>
        /// Updates position change image sprite, color and value for display depending on drivers current position compared to starting position
        /// </summary>
        void UpdatePositionChange(DriverData driverData, ref bool changed)
        {
            int positionChanged = driverData.LapData.gridPosition - driverData.LapData.carPosition;
            //No need to update values since they are the same
            if (positionChanged == _lastPositionChanged)
                return;
            changed = true;

            //Gained places
            if (positionChanged > 0)
                ChangePositionSprite(_positionChangedUp, _positionChangedUpColor, positionChanged);
            //Dropped places
            else if (positionChanged < 0)
                ChangePositionSprite(_positionChangedDown, _positionChangedDownColor, positionChanged);
            //Unchanged
            else
                ChangePositionSprite(_positionChangedUnchanged, _positionChangedUnchangedolor, positionChanged);

            _lastPositionChanged = positionChanged;
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

        /// <summary>
        /// Updates tyre type and laps old to display based on driver data
        /// </summary>
        void UpdateTyre(DriverData driverData, ref bool changed)
        {
            //No need to update values since they are the same
            if (_lastTyreCompound == driverData.StatusData.visualTyreCompound && _lastTyreLife == driverData.StatusData.tyreAgeInLaps)
                return;
            changed = true;

            _tyreImage.sprite = GameManager.ParticipantManager.GetVisualTyreCompoundSprite(driverData.StatusData.visualTyreCompound);
            if (driverData.ParticipantData.publicTelemetry)
                _tyreLapText.text = driverData.StatusData.tyreAgeInLaps.ToString() + _tyreLapEndingString;
            else
                _tyreLapText.text = _tyreLapUnavailableDataString + _tyreLapEndingString;

            _lastTyreCompound = driverData.StatusData.visualTyreCompound;
            _lastTyreLife = driverData.StatusData.tyreAgeInLaps;
        }

        /// <summary>
        /// Updates text for amount of stops driver has currently done in the race
        /// </summary>
        void UpdateStop(DriverData driverData, ref bool changed)
        {
            byte timesPitted = GameManager.LapManager.TimesPitted(driverData.VehicleIndex);
            //No need to update values since they are the same
            if (timesPitted == _lastAmountOfStops)
                return;
            changed = true;

            _stopText.text = timesPitted + _stopEndingString;

            _lastAmountOfStops = timesPitted;
        }

        /// <summary>
        /// Updates sprite and text for penalty situation for driver
        /// </summary>
        void UpdatePenalty(DriverData driverData, ref bool changed)
        {
            //No need to update values since they are the same
            int totalPenalties = driverData.LapData.totalPenalties;
            if (totalPenalties == _lastAmountOfPenalties)
                return;
            changed = true;

            _penaltyImage.gameObject.SetActive(totalPenalties != 0);
            if (totalPenalties > 0)
                _penaltyText.text = totalPenalties.ToString() + _penaltyEndingString;
            else
                _penaltyText.text = string.Empty;

            _lastAmountOfPenalties = totalPenalties;
        }

        #endregion

        #region Structs

        /// <summary>
        /// What sort of information should be displayed -> penalty symbol will always show
        /// </summary>
        public enum TimingStatsState
        {
            None,
            Position_Changed,
            Tyre,
            Stop,
            Penalty,
            All,
            Length,
        }

        #endregion
    }
}