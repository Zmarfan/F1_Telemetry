using UnityEngine;
using UnityEngine.UI;
using F1_Data_Management;

namespace F1_Unity
{
    public class LiveSpeed : MonoBehaviour
    {
        [SerializeField] Color _slowColor;
        [SerializeField] Color _fastColor;
        [SerializeField] int _transitionToFastColorSpeedKMH = 200;

        [SerializeField] CanvasGroup _canvasGroup;
        [SerializeField] Text _driverText;
        [SerializeField] Text _kmhSpeedText;
        [SerializeField] Text _mphSpeedText;

        private void Update()
        {
            if (GameManager.F1Info.ReadyToReadFrom)
                UpdateValues();
            else
                Show(false);
        }

        /// <summary>
        /// Sets speed and driver name every frame this object is activated
        /// </summary>
        void UpdateValues()
        {
            DriverData spectatorDriverData = GameManager.F1Info.ReadSpectatingCarData(out bool status);
            if (status)
            {
                Show(true);
                //Speed is in kmh so convert to mph as well
                ushort speedInKMH = spectatorDriverData.TelemetryData.speed;
                ushort speedInMPH = (ushort)(speedInKMH * Constants.CONVERT_KMH_TO_MPH);


                _kmhSpeedText.text = speedInKMH.ToString();
                _mphSpeedText.text = speedInMPH.ToString();

                _driverText.text = GameManager.ParticipantManager.GetNameFromNumber(spectatorDriverData.RaceNumber).ToUpper();

                SetColor(speedInKMH);
            }
            else
                Show(false);
        }

        /// <summary>
        /// Sets color on text based on speed
        /// </summary>
        void SetColor(int speed)
        {
            if (speed >= _transitionToFastColorSpeedKMH)
            {
                _kmhSpeedText.color = _fastColor;
                _mphSpeedText.color = _fastColor;
            }
            else
            {
                _kmhSpeedText.color = _slowColor;
                _mphSpeedText.color = _slowColor;
            }
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

