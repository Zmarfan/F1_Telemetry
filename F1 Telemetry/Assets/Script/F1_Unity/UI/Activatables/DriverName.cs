using UnityEngine;
using F1_Data_Management;
using UnityEngine.UI;

namespace F1_Unity
{
    public class DriverName : MonoBehaviour, IActivatableReset
    {
        [Header("Settings")]

        [SerializeField] protected CanvasGroup _canvasGroup;
        [SerializeField, Range(0.0f, 1.0f)] protected float _raceNumberColorAlpha = 0.45f;
        [SerializeField] protected Text _positionText;
        [SerializeField] protected Image _teamColorImage;
        [SerializeField] protected Text _driverNameText;
        [SerializeField] protected Text _raceNumberText;
        [SerializeField] protected Shadow _raceNumberShadow;

        [Header("Optional")]

        [SerializeField] protected Text _teamNameText;
        [SerializeField] protected Image _flagImage;

        protected byte _currentDriverId = byte.MaxValue;
        protected byte _currentDriverPosition = byte.MaxValue;

        public void ClearActivatable()
        {
            _currentDriverId = byte.MaxValue;
            _currentDriverPosition = byte.MaxValue;
        }

        private void Update()
        {
            if (GameManager.F1Info.ReadyToReadFrom)
                UpdateVisuals();
            else
                Show(false);
        }

        /// <summary>
        /// Runs once per frame -> is used to set one time things and for frame operations if needed
        /// </summary>
        protected virtual void UpdateVisuals()
        {
            DriverData spectatorDriverData = GameManager.F1Info.ReadSpectatingCarData(out bool statusDriver);

            if (statusDriver && (_currentDriverId != spectatorDriverData.ID || _currentDriverPosition != spectatorDriverData.LapData.carPosition))
            {
                Show(true);
                SetVisuals(spectatorDriverData);
            }
            else if (!statusDriver)
                Show(false);
        }

        /// <summary>
        /// Sets the visuals
        /// </summary>
        protected virtual void SetVisuals(DriverData spectatorDriverData)
        {
            _currentDriverId = spectatorDriverData.ID;
            _currentDriverPosition = spectatorDriverData.LapData.carPosition;

            _positionText.text = spectatorDriverData.LapData.carPosition.ToString();

            Color color = GameManager.F1Utility.GetColorByTeam(spectatorDriverData.ParticipantData.team);
            _teamColorImage.color = color;
            color.a = _raceNumberColorAlpha;
            _raceNumberShadow.effectColor = color;

            _driverNameText.text = GameManager.ParticipantManager.GetNameFromNumber(spectatorDriverData.RaceNumber);
            _raceNumberText.text = "<i>" + spectatorDriverData.RaceNumber + "</i>"; //Puts it in italics

            //Seperated for children that don't have these
            if (_teamNameText != null)
                _teamNameText.text = ConvertEnumToString.Convert<Team>(spectatorDriverData.ParticipantData.team);
            if (_flagImage != null)
                _flagImage.sprite = GameManager.FlagManager.GetFlag(spectatorDriverData.ParticipantData.nationality);
        }

        /// <summary>
        /// Show or hide activatable
        /// </summary>
        protected void Show(bool status)
        {
            _canvasGroup.alpha = status ? 1.0f : 0.0f;
        }
    }
}