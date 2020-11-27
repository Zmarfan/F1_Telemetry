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
        [SerializeField] string _preNumberString = "LAP ";
        [SerializeField] Text _currentLapText;
        [SerializeField] Text _totalLapText;
        [SerializeField] DriverTemplate _leaderTemplate;

        byte _currentLap = 0;
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
                _totalLapText.text = sessionData.TotalLaps.ToString();
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
            byte lap = _leaderTemplate.DriverData.LapData.currentLapNumber;
            if (lap != _currentLap)
            {
                _currentLap = lap;
                _currentLapText.text = _preNumberString + lap.ToString();
            }
        }
    }
}