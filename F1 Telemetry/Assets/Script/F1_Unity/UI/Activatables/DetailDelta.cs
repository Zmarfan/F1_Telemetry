using UnityEngine;
using UnityEngine.UI;
using F1_Data_Management;
using System;
using System.Text;

namespace F1_Unity
{
    public class DetailDelta : MonoBehaviour
    {
        [Header("Settings")]

        [SerializeField, Range(0.01f, 10f)] float _timeBetweenDeltaUpdates = 1.0f; 
        [SerializeField] string _defaultDeltaString = "---";
        [SerializeField] Color _startingDeltaColor = Color.grey;
        [SerializeField] Color _slowerColor = Color.red;
        [SerializeField] Color _fasterColor = Color.green;

        [Header("Drop")]

        [SerializeField] Text _deltaText;
        [SerializeField] CanvasGroup _canvasGroup;
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

        Timer _deltaTimer;
        float _lastDelta;

        private void Awake()
        {
            _deltaTimer = new Timer(_timeBetweenDeltaUpdates);
        }

        private void Update()
        {
            if (GameManager.F1Info.ReadyToReadFrom)
                UpdateDriverDelta();
        }

        void UpdateDriverDelta()
        {
            _deltaTimer.Time += Time.deltaTime;

            bool status1 = false;
            bool status2 = false;
            DriverData d2Data = GameManager.F1Info.ReadSpectatingCarData(out status1);

            if (status1)
            {
                //Getting driverData of driver in front of spectating car
                if (d2Data.LapData.carPosition - 2 >= 0)
                {
                    status2 = true;
                    DriverData d1Data = _driverTemplates[d2Data.LapData.carPosition - 2].DriverData;
                    SetData(d1Data, d2Data, d2Data.LapData.carPosition - 1);
                }
                //It's the leader
                else
                {
                    status2 = true;
                    //Read car behind spectator if he is leading
                    DriverData d1Data = _driverTemplates[d2Data.LapData.carPosition].DriverData;
                    SetData(d2Data, d1Data, d2Data.LapData.carPosition);
                }
            }

            if (status1 && status2)
                Show(true);
            else
                Show(false);
        }

        /// <summary>
        /// Sets delta and driver details if needed.
        /// </summary>
        void SetData(DriverData d1Data, DriverData d2Data, int deltaIndex)
        {
            //Not showing correct driver info -> fix that
            if (_driver1ID != d1Data.ID || _driver2ID != d2Data.ID)
            {
                _driver1ID = d1Data.ID;
                _driver2ID = d2Data.ID;
                SetVisuals(d1Data, _driver1PositionText, _driver1NameText, _driver1NumberText, _driver1CarImage, _driver1TeamImage, _driver1PortraitImage, _driver1TeamStripeImage);
                SetVisuals(d2Data, _driver2PositionText, _driver2NameText, _driver2NumberText, _driver2CarImage, _driver2TeamImage, _driver2PortraitImage, _driver2TeamStripeImage);

                UpdateDelta(deltaIndex);
            }

            if (_deltaTimer.Expired())
                UpdateDelta(deltaIndex);
        }

        /// <summary>
        /// If there is no valid data to look at -> Don't show any
        /// </summary>
        void Show(bool active)
        {
            _canvasGroup.alpha = active ? 1.0f : 0.0f;
        }

        /// <summary>
        /// Sets the delta.
        /// </summary>
        void UpdateDelta(int index)
        {
            _deltaTimer.Reset();

            //We don't want to set anything if the delta isn't yet correct
            DriverTimeState state = _driverTemplates[index].TimeState;
            if (state != DriverTimeState.Starting && state != DriverTimeState.Pit && state != DriverTimeState.Pit_Area && state != DriverTimeState.Lapped)
            {
                float delta = _driverTemplates[index].DeltaToCarInFront;
                TimeSpan span = TimeSpan.FromSeconds(delta);

                StringBuilder builder = new StringBuilder();

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

                //Color
                _deltaText.color = delta > _lastDelta ? _slowerColor : _fasterColor;

                _lastDelta = delta;
            }
            else
            {
                _deltaText.color = _startingDeltaColor;
                _deltaText.text = _defaultDeltaString;
            }
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