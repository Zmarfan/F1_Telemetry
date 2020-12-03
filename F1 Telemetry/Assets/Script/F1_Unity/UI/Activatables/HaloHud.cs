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

        [SerializeField] Text _gForceText;
        [SerializeField, Range(0.01f, 5f)] float _gForceUpdateRate = 0.25f;
        [SerializeField] GForceDirection[] _gForceDirections;
        [SerializeField, Range(0.0f, 100f)] float _level0GForceMinForce = 0.3f; 
        [SerializeField, Range(0.0f, 100f)] float _level1GForceMinForce = 1.75f; 
        [SerializeField, Range(0.0f, 100f)] float _level2GForceMinForce = 2.5f; 
        [SerializeField, Range(0f, 500f)] float _highSpeedColorLimit = 300;
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

        Timer _gForceTimer;

        private void Awake()
        {
            _gForceTimer = new Timer(_gForceUpdateRate);
        }

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
                _gForceTimer.Time += Time.deltaTime;

                Show(true);
                UpdateSliders(driverData);
                UpdateDisplay(driverData);
                if (_gForceTimer.Expired())
                    UpdateGForce(driverData);
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
        /// Sets the G-force visuals for halo hud
        /// </summary>
        void UpdateGForce(DriverData driverData)
        {
            _gForceTimer.Reset();

            //x -> side to side, y -> forward/backward
            Vector2 gForce = new Vector2(driverData.MotionData.gForceLateral, -driverData.MotionData.gForceLongitudinal);
            float gForceMagnitude = gForce.magnitude;
            _gForceText.text = gForceMagnitude.ToString("#.0G").Replace(',', '.');

            //Visuals
            for (int i = 0; i < _gForceDirections.Length; i++)
            {
                bool level0 = false;
                bool level1 = false;
                bool level2 = false;
                Vector2 direction = _gForceDirections[i].direction;
                direction = direction.normalized * gForceMagnitude;

                float dot = Vector3.Dot(direction.normalized, gForce.normalized);
                //The force is along this direction in some form
                if (dot >= 0)
                {
                    Vector2 projection = Vector3.Project(direction, gForce);
                    float sqrMagnitude = projection.sqrMagnitude * dot * dot * dot;
                    level0 = sqrMagnitude >= _level0GForceMinForce * _level0GForceMinForce;
                    level1 = sqrMagnitude >= _level1GForceMinForce * _level1GForceMinForce;
                    level2 = sqrMagnitude >= _level2GForceMinForce * _level2GForceMinForce;
                }

                _gForceDirections[i].level0.SetActive(level0);
                _gForceDirections[i].level1.SetActive(level1);
                _gForceDirections[i].level2.SetActive(level2);
            }
        }

        /// <summary>
        /// Shows or hides the activatable
        /// </summary>
        void Show(bool status)
        {
            _canvasGroup.alpha = status ? 1.0f : 0.0f;
        }

        /// <summary>
        /// Holds Vector of G-Force direction and the gameobjects affected by that direction
        /// </summary>
        [System.Serializable]
        public struct GForceDirection
        {
            [Header("Only for debugging purpose")]
            public string header;
            [Header("Settings")]
            public Vector2 direction;
            public GameObject level0;
            public GameObject level1;
            public GameObject level2;
        }
    }
}