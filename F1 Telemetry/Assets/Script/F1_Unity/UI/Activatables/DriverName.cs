﻿using UnityEngine;
using F1_Data_Management;
using UnityEngine.UI;

namespace F1_Unity
{
    public class DriverName : MonoBehaviour
    {
        [SerializeField, Range(0.0f, 1.0f)] float _raceNumberColorAlpha = 0.45f;
        [SerializeField] Text _positionText;
        [SerializeField] Image _teamColorImage;
        [SerializeField] Text _driverNameText;
        [SerializeField] Text _teamNameText;
        [SerializeField] Text _raceNumberText;
        [SerializeField] Shadow _raceNumberShadow;
        [SerializeField] Image _flagImage;

        byte _currentDriverId = byte.MaxValue;

        private void Update()
        {
            if (GameManager.F1Info.ReadyToReadFrom)
                UpdateVisuals();
        }

        void UpdateVisuals()
        {
            DriverData spectatorDriverData = GameManager.F1Info.ReadSpectatingCarData(out bool status);

            if (status && _currentDriverId != spectatorDriverData.ID)
            {
                _currentDriverId = spectatorDriverData.ID;

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
        }
    }
}