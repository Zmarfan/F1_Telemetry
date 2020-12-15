using System.Collections.Generic;
using UnityEngine;
using F1_Data_Management;
using System.Linq;

namespace F1_Unity
{
    /// <summary>
    /// Holds a dictionary of all drivers matched with their position as key. Easy access across program.
    /// </summary>
    public class DriverDataManager : MonoBehaviour
    {
        Dictionary<int, DriverData> _positionToData = new Dictionary<int, DriverData>();
        /// <summary>
        /// Holds data for every driver currently entered in the championship
        /// </summary>
        Dictionary<int, ChampionshipEntry> _championshipDictionary = new Dictionary<int, ChampionshipEntry>();

        /// <summary>
        /// Sets all lists that user have control over, is called before Awake is called
        /// </summary>
        public void Init(List<ChampionshipEntry> championshipPointList)
        {
            List<ChampionshipEntry> sortedList = championshipPointList.OrderByDescending(item => item.points).ToList();
            //Sets the position of all drivers
            for (int i = 0; i < sortedList.Count; i++)
            {
                ChampionshipEntry entry = sortedList[i];
                entry.position = i + 1;
                sortedList[i] = entry;
            }

            //Sets dictionary from list -> better access
            for (int i = 0; i < sortedList.Count; i++)
                _championshipDictionary.Add(sortedList[i].raceNumber, sortedList[i]);
        }

        /// <summary>
        /// Gets information about driver in the championship based on racing number
        /// </summary>
        /// <param name="racingNumber">Number of driver</param>
        /// <param name="status">Indicates if returned data is valid or junk</param>
        public ChampionshipEntry GetChampionShipEntry(int racingNumber, out bool status)
        {
            try
            {
                status = true;
                return _championshipDictionary[racingNumber];
            }
            catch
            {
                status = false;
                return new ChampionshipEntry();
            }
        }

        /// <summary>
        /// Returns driverData given a position. Will not check if it's a valid position so make sure it is.
        /// </summary>
        /// <param name="position">position of driver</param>
        /// <returns></returns>
        public DriverData GetDriverFromPosition(int position, out bool status)
        {
            try
            {
                status = true;
                return _positionToData[position];
            }
            catch
            {
                status = false;
                return new DriverData();
            }
        }

        private void Update()
        {
            if (GameManager.F1Info.ReadyToReadFrom)
                UpdatePositionToData();
        }

        /// <summary>
        /// Updates the dictionary so it's up to date.
        /// </summary>
        void UpdatePositionToData()
        {
            _positionToData.Clear();
            for (int i = 0; i < F1Info.MAX_AMOUNT_OF_CARS; i++)
            {
                DriverData driverData = GameManager.F1Info.ReadCarData(i, out bool status);
                if (status)
                    _positionToData.Add(driverData.LapData.carPosition, driverData);
            }
        }

        [System.Serializable]
        public struct ChampionshipEntry
        {
            /// <summary>
            /// Number of current amount of championship points
            /// </summary>
            public int points;
            /// <summary>
            /// Race number for the driver 
            /// </summary>
            public byte raceNumber;
            /// <summary>
            /// Current position in championship
            /// </summary>
            [HideInInspector] public int position; //Calculated later
        }
    }
}