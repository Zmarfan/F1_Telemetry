using System.Collections.Generic;
using UnityEngine;
using F1_Data_Management;

namespace F1_Unity
{
    /// <summary>
    /// Holds a dictionary of all drivers matched with their position as key. Easy access across program.
    /// </summary>
    public class DriverDataManager : MonoBehaviour
    {
        static DriverDataManager _singleton;
        static Dictionary<int, DriverData> _positionToData = new Dictionary<int, DriverData>();

        private void Awake()
        {
            if (_singleton == null)
                _singleton = this;
            else
                Destroy(this.gameObject);
        }

        /// <summary>
        /// Returns driverData given a position. Will not check if it's a valid position so make sure it is.
        /// </summary>
        /// <param name="position">position of driver</param>
        /// <returns></returns>
        public static DriverData GetDriverFromPosition(int position, out bool status)
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
            for (int i = 0; i < F1Info.MAX_AMOUNT_OF_CARS; i++)
            {
                DriverData driverData = GameManager.F1Info.ReadCarData(i, out bool status);
                if (status)
                    _positionToData.Add(driverData.LapData.carPosition, driverData);
            }
        }
    }
}