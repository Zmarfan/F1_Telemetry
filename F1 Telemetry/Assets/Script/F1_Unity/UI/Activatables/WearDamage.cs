using UnityEngine;
using F1_Data_Management;
using UnityEngine.UI;

namespace F1_Unity
{
    /// <summary>
    /// Displays tyre wear and wing damage for spectating car if telemetry is public
    /// </summary>
    public class WearDamage : MonoBehaviour, IActivatableReset
    {
        [Header("Settings")]

        [SerializeField] Gradient _damageGradient;
        [SerializeField] Color _hiddenTelemetryTyreColor;
        [SerializeField] string _hiddenTelemetryTyreText = "N/A";
        [SerializeField] CanvasGroup _canvasGroup;

        [Header("Car Drop")]

        [SerializeField] Text _driverNameText;
        [SerializeField] Image _teamColorImage;
        [SerializeField] Text[] _wheelPercentageTexts;   //Order of Wheel class for tyres -> easier to work with
        [SerializeField] Image[] _wheelPercentageImages; //Order of Wheel class for tyres -> easier to work with
        [SerializeField] Text _leftWingPercentageText;
        [SerializeField] Text _rightWingPercentageText;
        [SerializeField] Image _leftWingPercentageImage;
        [SerializeField] Image _rightWingPercentageImage;

        [Header("Lower Drop")]

        [SerializeField] Image _tyreTypeImage;
        [SerializeField] Text _tyreTypeText;

        byte _driverID = byte.MaxValue;

        public void ClearActivatable()
        {
            _driverID = byte.MaxValue;
        }

        private void Update()
        {
            if (GameManager.F1Info.ReadyToReadFrom)
                UpdateWearDamage();
            else
                _canvasGroup.alpha = 0.0f;
        }

        /// <summary>
        /// Updates the visual values to match spectating car
        /// </summary>
        void UpdateWearDamage()
        {
            DriverData driverData = GameManager.F1Info.ReadSpectatingCarData(out bool status);
            if (status)
            {
                //New Target
                if (_driverID != driverData.ID)
                {
                    _driverID = driverData.ID;
                    _canvasGroup.alpha = 1.0f;
                    SetIndDriverVisuals(driverData);
                }
                //Update
                SetFrameVisuals(driverData);
            }
            //Currently not spectating -> hide
            else
                _canvasGroup.alpha = 0.0f;
        }

        /// <summary>
        /// Values updates every frame to be kept up to date.
        /// </summary>
        void SetFrameVisuals(DriverData driverData)
        {
            //Tyre compound
            _tyreTypeImage.sprite = GameManager.ParticipantManager.GetVisualTyreCompoundSprite(driverData.StatusData.visualTyreCompound);
            _tyreTypeText.text = ConvertEnumToString.Convert<VisualTyreCompound>(driverData.StatusData.visualTyreCompound).ToUpper();

            //Some drivers have hidden telemetry -> Can't access those values
            if (driverData.ParticipantData.publicTelemetry)
            {
                //Tyres
                for (int i = 0; i < _wheelPercentageImages.Length; i++)
                    SetDamage(driverData.StatusData.tyreWear[i], _wheelPercentageImages[i], _wheelPercentageTexts[i]);
                //Wings
                SetDamage(driverData.StatusData.frontLeftWingDamage, _leftWingPercentageImage, _leftWingPercentageText);
                SetDamage(driverData.StatusData.frontRightWingDamage, _rightWingPercentageImage, _rightWingPercentageText);
            }
            //Show blank screen for those with hidden telemetry
            else
            {
                //Tyres
                for (int i = 0; i < _wheelPercentageImages.Length; i++)
                {
                    _wheelPercentageImages[i].color = _hiddenTelemetryTyreColor;
                    _wheelPercentageTexts[i].text = _hiddenTelemetryTyreText;
                }
                //Wings
                _leftWingPercentageImage.color = _hiddenTelemetryTyreColor;
                _rightWingPercentageImage.color = _hiddenTelemetryTyreColor;
                _leftWingPercentageText.text = _hiddenTelemetryTyreText;
                _rightWingPercentageText.text = _hiddenTelemetryTyreText;
            }
        }

        /// <summary>
        /// Sets color and percentage for a tyre wear.
        /// </summary>
        void SetDamage(byte wear, Image image, Text text)
        {
            text.text = wear + "%";
            float fraction = wear / 100f;
            image.color = _damageGradient.Evaluate(fraction);
        }

        /// <summary>
        /// Sets the visuals that won't change each updata -> driver name
        /// </summary>
        void SetIndDriverVisuals(DriverData driverData)
        {
            _driverNameText.text = GameManager.ParticipantManager.GetNameFromNumber(driverData.RaceNumber).ToUpper();
            _teamColorImage.color = GameManager.F1Utility.GetColorByTeam(driverData.ParticipantData.team);
        }
    }
}