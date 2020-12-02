using UnityEngine;
using F1_Data_Management;
using UnityEngine.UI;

namespace F1_Unity
{
    public class HaloHud : MonoBehaviour
    {
        const ushort MAX_RPM = 15000;
        const sbyte REVERSE = -1;
        const sbyte NEUTRAL = 0;

        [Header("Drop")]

        [SerializeField] CanvasGroup _canvasGroup;

        [Header("Display")]

        [SerializeField] float _highSpeedColorLimit = 300;
        [SerializeField] Color _slowSpeedColor = Color.white;
        [SerializeField] Color _highSpeedColor;
        [SerializeField] string _neutralText = "N";
        [SerializeField] string _reverseText = "R";
        [SerializeField] Text _gearText;
        [SerializeField] Text _kmhText;
        [SerializeField] Text _mphText;

        [Header("Sliders")]

        [SerializeField] Image _throttleBackground;
        [SerializeField] Slider _throttleHandleSlider;
        [SerializeField] Image _brakeBackground;
        [SerializeField] Slider _brakeHandleSlider;
        [SerializeField] Image _rpmBackground;

        private void Update()
        {
            if (GameManager.F1Info.ReadyToReadFrom)
                UpdateHaloHud();
            else
                Show(false);
        }

        /// <summary>
        /// Updates each aspect of the halohud
        /// </summary>
        void UpdateHaloHud()
        {
            DriverData driverData = GameManager.F1Info.ReadSpectatingCarData(out bool status);
            if (status)
            {
                Show(true);
                UpdateSliders(driverData);
                UpdateDisplay(driverData);
            }
            else
                Show(false);
        }

        /// <summary>
        /// Updates the visuals for throttle, RPM and brake sliders in halo hud
        /// </summary>
        void UpdateSliders(DriverData driverData)
        {
            //Throttle
            _throttleBackground.fillAmount = driverData.TelemetryData.throttle;
            _throttleHandleSlider.value = driverData.TelemetryData.throttle;
            //Brake
            _brakeBackground.fillAmount = driverData.TelemetryData.brake;
            _brakeHandleSlider.value = driverData.TelemetryData.brake;
            //RPM
            ushort rpm = driverData.TelemetryData.engineRPM;
            float fraction = rpm / (float)MAX_RPM;
            _rpmBackground.fillAmount = fraction;
        }

        /// <summary>
        /// Updates the visuals in top part of halo hud -> speed and gear
        /// </summary>
        void UpdateDisplay(DriverData driverData)
        {
            //Gear
            switch (driverData.TelemetryData.gear)
            {
                case (REVERSE): _gearText.text = _reverseText; break;
                case (NEUTRAL): _gearText.text = _neutralText; break;
                default: _gearText.text = driverData.TelemetryData.gear.ToString(); break;
            }
            //Speed
            ushort kmhSpeed = driverData.TelemetryData.speed;
            ushort mphSpeed = (ushort)(kmhSpeed * Constants.CONVERT_KMH_TO_MPH);
            _kmhText.text = kmhSpeed.ToString();
            _mphText.text = mphSpeed.ToString();

            if (kmhSpeed >= _highSpeedColorLimit)
            {
                _kmhText.color = _highSpeedColor;
                _mphText.color = _highSpeedColor;
            }
            else
            {
                _kmhText.color = _slowSpeedColor;
                _mphText.color = _slowSpeedColor;
            }
        }

        /// <summary>
        /// Shows or hides the activatable
        /// </summary>
        void Show(bool status)
        {
            _canvasGroup.alpha = status ? 1.0f : 0.0f;
        }
    }
}