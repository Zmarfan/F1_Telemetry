using UnityEngine;
using UnityEngine.UI;
using F1_Data_Management;

namespace F1_Unity
{
    public class WeatherInfo : MonoBehaviour
    {
        [SerializeField] Image _weatherImage;
        [SerializeField] Text _airTempCText;
        [SerializeField] Text _airTempFText;
        [SerializeField] Text _trackTempCText;
        [SerializeField] Text _trackTempFText;

        private void OnEnable()
        {
            Session sessionData = GameManager.F1Info.ReadSession(out bool status);
            if (status)
            {
                _weatherImage.sprite = FlagManager.GetWeatherSprite(sessionData.Weather);
                SetTemperatureTexts(sessionData.AirTemperature, _airTempCText, _airTempFText);
                SetTemperatureTexts(sessionData.TrackTemperature, _trackTempCText, _trackTempFText);
            }
            else
                gameObject.SetActive(false);
        }

        /// <summary>
        /// Calculates and set the temperature for input texts
        /// </summary>
        void SetTemperatureTexts(sbyte temperatureC, Text tempCText, Text tempFText)
        {
            sbyte airTempF = (sbyte)Constants.ConvertCelsiusToFahrenheit(temperatureC);
            tempCText.text = temperatureC.ToString();
            tempFText.text = airTempF.ToString();
        }
    }
}