using UnityEngine;
using F1_Data_Management;
using UnityEngine.UI;

namespace F1_Unity
{
    /// <summary>
    /// Displays current lapNumber / totalLaps
    /// </summary>
    public class LapDisplay : MonoBehaviour
    {
        [SerializeField] Text _currentLapText;
        [SerializeField] Text _totalLapText;
        [SerializeField] Image _displayImage;
        [SerializeField] Sprite _defaultDisplaySprite;
        [SerializeField] Sprite _raceFinishedDisplaySprite;

        byte _currentLap = 0;
        byte _totalLap = 0;
        bool _init = true;

        /// <summary>
        /// Sets total laps
        /// </summary>
        void Init()
        {
            Session sessionData = GameManager.F1Info.ReadSession(out bool status);
            if (status)
            {
                _init = false;
                _totalLap = sessionData.TotalLaps;
                _totalLapText.text = _totalLap.ToString();
            }
        }

        private void Update()
        {
            if (GameManager.F1Info.ReadyToReadFrom)
            {
                if (_init)
                    Init();
                else
                    UpdateLap();
            }      
        }

        /// <summary>
        /// Called every frame to check if it's time to change lap
        /// </summary>
        void UpdateLap()
        {
            byte lap = GameManager.DriverDataManager.GetDriverFromPosition(1, out bool status).LapData.currentLapNumber;
            //Change lap if new lap and it's not larger than total laps
            if (status && lap != _currentLap)
            {
                _currentLap = lap;
                if (lap <= _totalLap)
                    _currentLapText.text = lap.ToString();

                if (_currentLap > _totalLap)
                    _displayImage.sprite = _raceFinishedDisplaySprite;
                else
                    _displayImage.sprite = _defaultDisplaySprite;
            }
        }
    }
}