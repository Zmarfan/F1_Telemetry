using UnityEngine;
using F1_Data_Management;
using UnityEngine.UI;

namespace F1_Unity
{
    public class DriverName : MonoBehaviour
    {
        [SerializeField] CanvasGroup _canvasGroup;
        [SerializeField, Range(0.0f, 1.0f)] float _raceNumberColorAlpha = 0.45f;
        [SerializeField] Text _positionText;
        [SerializeField] Image _teamColorImage;
        [SerializeField] Text _driverNameText;
        [SerializeField] Text _teamNameText;
        [SerializeField] Text _raceNumberText;
        [SerializeField] Shadow _raceNumberShadow;
        [SerializeField] Image _flagImage;

        byte _currentDriverId = byte.MaxValue;
        byte _currentDriverPosition = byte.MaxValue;

        private void Update()
        {
            if (GameManager.F1Info.ReadyToReadFrom)
                UpdateVisuals();
            else
                Show(false);
        }

        void UpdateVisuals()
        {
            Session sessionData = GameManager.F1Info.ReadSession(out bool statusSession);
            DriverData spectatorDriverData = GameManager.F1Info.ReadSpectatingCarData(out bool statusDriver);

            if (statusDriver && (_currentDriverId != spectatorDriverData.ID || _currentDriverPosition != spectatorDriverData.LapData.carPosition))
            {
                Show(true);
                _currentDriverId = spectatorDriverData.ID;
                _currentDriverPosition = spectatorDriverData.LapData.carPosition;

                _positionText.text = spectatorDriverData.LapData.carPosition.ToString();

                Color color = TeamColor.GetColorByTeam(spectatorDriverData.ParticipantData.team);
                _teamColorImage.color = color;
                color.a = _raceNumberColorAlpha;
                _raceNumberShadow.effectColor = color;

                _driverNameText.text = ParticipantManager.GetNameFromNumber(spectatorDriverData.RaceNumber);
                _raceNumberText.text = "<i>" + spectatorDriverData.RaceNumber + "</i>"; //Puts it in italics
                _teamNameText.text = ConvertEnumToString.Convert<Team>(spectatorDriverData.ParticipantData.team);
                _flagImage.sprite = FlagManager.GetFlag(spectatorDriverData.ParticipantData.nationality);
            }
            else if (!sessionData.IsSpectating)
                Show(false);
        }

        /// <summary>
        /// Show or hide activatable
        /// </summary>
        void Show(bool status)
        {
            _canvasGroup.alpha = status ? 1.0f : 0.0f;
        }
    }
}