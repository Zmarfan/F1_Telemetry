using UnityEngine;
using UnityEngine.UI;
using F1_Data_Management;

namespace F1_Unity
{
    /// <summary>
    /// Compare the spectating car's speed and DRS usage with car ahead.
    /// </summary>
    public class SpeedCompare : ActivatableBase
    {
        [Header("Settings")]

        [SerializeField] Color _slowColor;
        [SerializeField] Color _fastColor;
        [SerializeField] Color _drsImageOnColor;
        [SerializeField] Color _drsImageOffColor;
        [SerializeField] Color _drsTextOnColor;
        [SerializeField] Color _drsTextOffColor;
        [SerializeField] int _transitionToFastColorSpeedKMH = 200;

        [Header("Left Driver")]

        [SerializeField] Image _driver1DRSImage;
        [SerializeField] Text _driver1DRSText;
        [SerializeField] Text _driver1KMHText;
        [SerializeField] Text _driver1MPHText;

        [Header("Right Driver")]

        [SerializeField] Image _driver2DRSImage;
        [SerializeField] Text _driver2DRSText;
        [SerializeField] Text _driver2KMHText;
        [SerializeField] Text _driver2MPHText;


        /// <summary>
        /// Sets delta and driver details if needed.
        /// </summary>
        protected override void SetData(DriverData d1Data, DriverData d2Data)
        {
            //Not showing correct driver info -> fix that
            if (_driver1ID != d1Data.ID || _driver2ID != d2Data.ID)
            {
                _driver1ID = d1Data.ID;
                _driver2ID = d2Data.ID;
                SetVisuals(d1Data, d2Data);
            }

            UpdateSpeed(d1Data, _driver1DRSImage, _driver1DRSText, _driver1KMHText, _driver1MPHText);
            UpdateSpeed(d2Data, _driver2DRSImage, _driver2DRSText, _driver2KMHText, _driver2MPHText);
        }

        /// <summary>
        /// Called every frame to update values
        /// </summary>
        void UpdateSpeed(DriverData driverData, Image DRSImage, Text DRSText, Text KMHText, Text MPHText)
        {
            //DRS
            bool drsON = driverData.TelemetryData.DRS;
            if (drsON)
            {
                DRSImage.color = _drsImageOnColor;
                DRSText.color = _drsTextOnColor;
            }
            else
            {
                DRSImage.color = _drsImageOffColor;
                DRSText.color = _drsTextOffColor;
            }

            //Speed
            ushort speedKMH = driverData.TelemetryData.speed;
            ushort speedMPH = (ushort)(speedKMH * Constants.CONVERT_KMH_TO_MPH);

            KMHText.text = speedKMH.ToString();
            MPHText.text = speedMPH.ToString();

            SetColor(speedKMH, KMHText, MPHText);
        }

        /// <summary>
        /// Sets color on text based on speed
        /// </summary>
        void SetColor(int speed, Text KMHText, Text MPHText)
        {
            if (speed >= _transitionToFastColorSpeedKMH)
            {
                KMHText.color = _fastColor;
                MPHText.color = _fastColor;
            }
            else
            {
                KMHText.color = _slowColor;
                MPHText.color = _slowColor;
            }
        }
    }
}