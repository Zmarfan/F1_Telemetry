using UnityEngine;
using F1_Data_Management;
using UnityEngine.UI;

namespace F1_Unity
{
    public class DriverNameChampionship : DriverName
    {
        [SerializeField] string _defaultValue = "N/A";
        [SerializeField] Text _positionInChampionshipText;
        [SerializeField] Text _thText;
        [SerializeField] Text _pointsInChampionshipText;

        protected override void SetVisuals(DriverData spectatorDriverData)
        {
            base.SetVisuals(spectatorDriverData);
            //Championship details
            var data = GameManager.DriverDataManager.GetChampionShipEntry(spectatorDriverData.RaceNumber, out bool status);
            //Could access data -> driver is in standings
            if (status)
            {
                _positionInChampionshipText.text = data.position.ToString();
                _thText.text = GetEndingType(data.position);
                _pointsInChampionshipText.text = data.points.ToString();
            }
            //Unknown driver -> default values
            else
            {
                _positionInChampionshipText.text = _defaultValue;
                _thText.text = string.Empty;
                _pointsInChampionshipText.text = _defaultValue;
            }
        }

        /// <summary>
        /// Returns the correct ending for a number -> st, nd, rd, th
        /// </summary>
        string GetEndingType(int number)
        {
            string num = number.ToString();
            if (num.EndsWith("11")) return "th";
            if (num.EndsWith("12")) return "th";
            if (num.EndsWith("13")) return "th";
            if (num.EndsWith("1")) return "st";
            if (num.EndsWith("2")) return "nd";
            if (num.EndsWith("3")) return "rd";
            return "th";
        }
    }
}