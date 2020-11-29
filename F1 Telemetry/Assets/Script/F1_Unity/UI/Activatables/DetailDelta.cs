using UnityEngine;
using UnityEngine.UI;
using F1_Data_Management;
using System;
using System.Text;

namespace F1_Unity
{
    public class DetailDelta : MonoBehaviour
    {
        [SerializeField] Text _deltaText;
        //Only used to get Delta as it is calculated and stored there -> no need to calculate again
        [SerializeField] DriverTemplate[] _driverTemplates;

        [Header("Left Driver")]

        [SerializeField] Text _driver1PositionText;
        [SerializeField] Text _driver1NameText;
        [SerializeField] Text _driver1NumberText;
        [SerializeField] Image _driver1CarImage;
        [SerializeField] Image _driver1TeamImage;
        [SerializeField] Image _driver1PortraitImage;
        [SerializeField] Image _driver1TeamStripeImage;

        [Header("Right Driver")]

        [SerializeField] Text _driver2PositionText;
        [SerializeField] Text _driver2NameText;
        [SerializeField] Text _driver2NumberText;
        [SerializeField] Image _driver2CarImage;
        [SerializeField] Image _driver2TeamImage;
        [SerializeField] Image _driver2PortraitImage;
        [SerializeField] Image _driver2TeamStripeImage;

        //Used to check if it needs to update anything but Delta
        byte _driver1ID = byte.MaxValue;
        byte _driver2ID = byte.MaxValue;

        private void Update()
        {
            if (GameManager.F1Info.ReadyToReadFrom)
                UpdateDriverDelta();
        }

        void UpdateDriverDelta()
        {
            DriverData d2Data = GameManager.F1Info.ReadSpectatingCarData(out bool status1);

            if (status1)
            {
                //Getting driverData of driver in front of spectating car
                DriverData d1Data = DriverDataManager.GetDriverFromPosition(d2Data.LapData.carPosition - 1, out bool status2);
                //Both contain valid data now
                if (status2)
                {
                    //Not showing correct driver info -> fix that
                    if (_driver1ID != d1Data.ID || _driver2ID != d2Data.ID)
                    {
                        _driver1ID = d1Data.ID;
                        _driver2ID = d2Data.ID;
                        SetVisuals(d2Data, _driver1PositionText, _driver1NameText, _driver1NumberText, _driver1CarImage, _driver1TeamImage, _driver1PortraitImage, _driver1TeamStripeImage);
                        SetVisuals(d1Data, _driver2PositionText, _driver2NameText, _driver2NumberText, _driver2CarImage, _driver2TeamImage, _driver2PortraitImage, _driver2TeamStripeImage);
                    }
                    UpdateDelta(d2Data.LapData.carPosition - 1);
                }
            }
        }

        /// <summary>
        /// Sets the delta.
        /// </summary>
        void UpdateDelta(int index)
        {
            TimeSpan span = new System.TimeSpan((long)_driverTemplates[index].DeltaToCarInFront);
            StringBuilder builder = new System.Text.StringBuilder();

            if (span.Minutes > 0)
            {
                builder.Append(span.Minutes);
                builder.Append(':');
                builder.Append(span.Seconds.ToString("0#")); //Start with zero if one digit long
            }
            else
                builder.Append(span.Seconds);

            builder.Append('.');
            builder.Append(span.Milliseconds.ToString("000")); //Appends with 3 decimals

            _deltaText.text = builder.ToString();
        }

        /// <summary>
        /// Sets all visuals, only done when changing subjects to spectate.
        /// </summary>
        void SetVisuals(DriverData driverData, Text driverPositionText, Text driverNameText, Text driverNumberText, Image driverCarImage, Image driverTeamImage, Image driverPortraitImage, Image driverTeamStripeImage)
        {
            driverPositionText.text = driverData.LapData.carPosition.ToString();
            driverNameText.text = ParticipantManager.GetNameFromNumber(driverData.RaceNumber).ToUpper();
            driverNumberText.text = "<i>" + driverData.RaceNumber + "</i>";
            driverCarImage.sprite = ParticipantManager.GetCarSprite(driverData.ParticipantData.team);
            driverTeamImage.sprite = ParticipantManager.GetTeamSprite(driverData.ParticipantData.team);
            driverPortraitImage.sprite = ParticipantManager.GetPortraitFromNumber(driverData.RaceNumber);
            driverTeamStripeImage.color = TeamColor.GetColorByTeam(driverData.ParticipantData.team);
        }
    }
}