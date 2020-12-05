using F1_Data_Management;
using UnityEngine;
using UnityEngine.UI;

namespace F1_Unity
{
    public class ERSCompare : ActivatableBase
    {
        [Header("Settings")]

        [SerializeField] Color _notAvailableColor;
        [SerializeField] Color _defaultColor;
        [SerializeField] Color _overtakeColor;
        [SerializeField, Range(0.01f, 5f)] float _updateFrequency = 1f;
        [SerializeField] string _valueNotAvailable = "N/A";

        [Header("Driver 1")]

        [SerializeField] Image _driver1FillImage;
        [SerializeField] Text _driver1ERSText;
        [SerializeField] Text _driver1DeployedText;
        [SerializeField] Text _driver1HarvestedText;

        [Header("Driver 2")]

        [SerializeField] Image _driver2FillImage;
        [SerializeField] Text _driver2ERSText;
        [SerializeField] Text _driver2DeployedText;
        [SerializeField] Text _driver2HarvestedText;

        Timer _updateTimer;

        private void Awake()
        {
            _updateTimer = new Timer(_updateFrequency);
        }

        /// <summary>
        /// Sets visuals for ERS.
        /// </summary>
        protected override void SetData(DriverData d1Data, DriverData d2Data)
        {
            _updateTimer.Time += Time.deltaTime;

            //Not showing correct driver info -> fix that
            if (_driver1ID != d1Data.ID || _driver2ID != d2Data.ID)
            {
                _driver1ID = d1Data.ID;
                _driver2ID = d2Data.ID;
                SetVisuals(d1Data, d2Data);
            }

            if (_updateTimer.Expired())
                SetVisuals(d1Data, d2Data);
        }

        protected override void SetVisuals(DriverData d1Data, DriverData d2Data)
        {
            _updateTimer.Reset();
            base.SetVisuals(d1Data, d2Data);
            SetERSVisuals(d1Data, _driver1FillImage, _driver1ERSText, _driver1DeployedText, _driver1HarvestedText);
            SetERSVisuals(d2Data, _driver2FillImage, _driver2ERSText, _driver2DeployedText, _driver2HarvestedText);
        }

        /// <summary>
        /// Sets the visuals based on driverData for driver
        /// </summary>
        void SetERSVisuals(DriverData driverData, Image ersSliderImage, Text ers, Text deployed, Text harvested)
        {
            if (driverData.ParticipantData.publicTelemetry)
            {
                ersSliderImage.fillAmount = driverData.StatusData.PercentageOfERSRemaining / 100f;
                ers.text = driverData.StatusData.PercentageOfERSRemaining.ToString() + "%";
                deployed.text = driverData.StatusData.PercentageOfERSDeployedThisLap.ToString() + "%";
                harvested.text = driverData.StatusData.PercentageOfERSHarvestedThisLap.ToString() + "%";

                if (driverData.StatusData.ERSDeploymentMode == ERSDeploymentMode.Overtake)
                    ersSliderImage.color = _overtakeColor;
                else
                    ersSliderImage.color = _defaultColor;
            }
            //Can't access these values -> set default values
            else
            {
                ersSliderImage.fillAmount = 1.0f;
                ers.text = _valueNotAvailable;
                deployed.text = _valueNotAvailable;
                harvested.text = _valueNotAvailable;

                ersSliderImage.color = _notAvailableColor;
            }
        }
    }
}