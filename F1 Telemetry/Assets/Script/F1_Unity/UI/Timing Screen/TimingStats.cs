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
        [SerializeField] GameObject _stopGoObj;
        [SerializeField] GameObject _finishedFlagObj;

        //Used to keep track of when an update is needed -> no need to poll
        int _lastPositionChanged = int.MinValue;
        VisualTyreCompound _lastTyreCompound;
        int _lastAmountOfPenalties = int.MinValue;
        int _lastStopGoPenalties = 0;
        int _lastTyreLife = int.MinValue;
        int _lastAmountOfStops = int.MinValue;
        bool _hasDriveThrough = false;
        bool _hasFinished = false;

        bool _out = false;

        /// <summary>
        /// Changes timing state to show specific information
        /// </summary>
        /// <param name="state">What state to go to</param>
        public void ChangeState(TimingStatsState state)
        {
            //Don't show any stats if the driver is out
            if (_out)
                state = TimingStatsState.None;

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
                if (_out || (_allStatsObjects[i] != _penaltyObj && _allStatsObjects[i] != _stopGoObj))
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
        public void UpdateValues(DriverData driverData, TimingStatsState currentState)
        {
            //Driver is out
            if (driverData.LapData.resultStatus == ResultStatus.Retired || driverData.LapData.resultStatus == ResultStatus.Disqualified)
            {
                //Only do this once
                if (!_out)
                {
                    //Set penalties to min value to force penalty image update if needed when going from out to in
                    _lastAmountOfPenalties = int.MinValue;
                    ChangeState(TimingStatsState.None);
                    _out = true;
                }
                return;
            }
            //DriverTemplate was out -> turn back
            if (_out)
            {
                _out = false;
                ChangeState(currentState);
            }

            bool changed = false;
            if (driverData.LapData.resultStatus == ResultStatus.Finished)
                SetFinished(ref changed);
            else
            {
                UpdatePositionChange(driverData, ref changed);
                UpdateTyre(driverData, ref changed);
                UpdateStop(driverData, ref changed);
                UpdatePenalty(driverData, ref changed);
            }

            //Align everything correctly if changes have been made
            if (changed)
                UpdateLayoutGroup();
        }

        #region Update Functions

        /// <summary>
        /// Turn off all stats and shows finish flag
        /// </summary>
        /// <param name="changed">If a change from last update was made changed is true</param>
        void SetFinished(ref bool changed)
        {
            if (!_hasFinished)
            {
                changed = true;
                //Turn off all stats and turn on finish flag
                for (int i = 0; i < _allStatsObjects.Count; i++)
                    _allStatsObjects[i].SetActive(false);
                _finishedFlagObj.SetActive(true);
            }
        }

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
            //single digit lead with space
            string showNumber = Mathf.Abs(positionChange).ToString();
            if (showNumber.Length == 1)
                showNumber += " ";
            _positionChangedText.text = showNumber;
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
            {
                //Force to be 2 char long -> align better
                string tyreLife = driverData.StatusData.tyreAgeInLaps.ToString();
                if (tyreLife.Length == 1)
                    tyreLife += " ";
                _tyreLapText.text = tyreLife + _tyreLapEndingString;
            }
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
            byte stopGoPenalties = GameManager.LapManager.AmountOfStopGoPenalties(driverData.VehicleIndex);
            bool driveThrough = GameManager.LapManager.HasDriveThrough(driverData.VehicleIndex);
            if (stopGoPenalties == _lastStopGoPenalties && _hasDriveThrough == driveThrough && totalPenalties == _lastAmountOfPenalties)
                return;

            changed = true;

            _penaltyObj.SetActive(totalPenalties != 0);
            //Set investigation symbol for drivers with drive through or stop go penalties

            _stopGoObj.SetActive(stopGoPenalties > 0 || driveThrough);
            if (totalPenalties > 0)
                _penaltyText.text = totalPenalties.ToString() + _penaltyEndingString;
            else
                _penaltyText.text = string.Empty;

            _lastAmountOfPenalties = totalPenalties;
            _lastStopGoPenalties = stopGoPenalties;
            _hasDriveThrough = driveThrough;
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
        }

        #endregion
    }
}