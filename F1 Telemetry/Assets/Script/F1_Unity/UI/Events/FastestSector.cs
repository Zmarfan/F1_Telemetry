using F1_Data_Management;
using UnityEngine;
using UnityEngine.UI;

namespace F1_Unity
{
    public class FastestSector : EventBase
    {
        [SerializeField] Text _sectorText;
        [SerializeField] Text _driverText;
        [SerializeField] Text _timeText;

        /// <summary>
        /// Called to start the event -> assign values and start fading procedure
        /// </summary>
        public void Init(DriverData driverData, LapState sector, float time)
        {
            Init();

            _sectorText.text = ConvertEnumToString.Convert(sector);
            _driverText.text = GameManager.ParticipantManager.GetNameFromNumber(driverData.RaceNumber).ToUpper();
            _timeText.text = F1Utility.GetDeltaString(time);
        }
    }
}