using System.Collections.Generic;
using UnityEngine;
using F1_Data_Management;

namespace F1_Unity
{
    public class TimingScreen : MonoBehaviour
    {
        [SerializeField, Range(0.01f, 5f)] float _flashColorDuration = 1.0f;
        [SerializeField] UnityEngine.Color _movedUpColor = UnityEngine.Color.green;
        [SerializeField] UnityEngine.Color _movedDownColor = UnityEngine.Color.red;
        [SerializeField] DriverTemplate[] _driverTemplates;

        static TimingScreen _singleton;

        //Reach driver position by their ID
        Dictionary<byte, int> _driverPosition;
        bool _initValues = true;

        private void Awake()
        {
            if (_singleton == null)
                Init();
            else
                Destroy(this.gameObject);
        }

        /// <summary>
        /// Assign correct number to each placement and create singleton
        /// </summary>
        void Init()
        {
            _singleton = this;
            for (int i = 0; i < _driverTemplates.Length; i++)
                _driverTemplates[i].Init(i + 1, _flashColorDuration);
        }

        /// <summary>
        /// Maps driver IDs to position on track. Used to compare old position to current
        /// </summary>
        void InitDrivers()
        {
            //Values are now initiated
            _initValues = false;

            _driverPosition = new Dictionary<byte, int>();

            for (int i = 0; i < F1Info.MAX_AMOUNT_OF_CARS; i++)
            {
                bool validDriver;
                DriverData driverData = GameManager.F1Info.ReadCarData(i, out validDriver);

                //Init everyone with gaining a position on start!
                //Flash green then to white
                //Only add valid drivers to the grid
                if (validDriver)
                    _driverPosition.Add(driverData.ParticipantData.driverID, driverData.LapData.carPosition + 1);

                //Only enable so many positions in time standing as there are active drivers
                //If they DNF/DSQ later they will only gray out, not be removed
                if (i >= GameManager.F1Info.ActiveDrivers)
                    _driverTemplates[i].SetActive(false);
                else
                    _driverTemplates[i].SetActive(true);
            }
        }

        //Updates standing
        private void Update()
        {
            //ONLY IN RACE!

            //Only update standings when data can be read safely and correctly
            if (GameManager.F1Info.ReadyToReadFrom)
            {
                if (_initValues)
                    InitDrivers();
                else
                    DoTimingScreen();
            }
        }

        /// <summary>
        /// Updates positions in standing and checks stability
        /// </summary>
        void DoTimingScreen()
        {
            //Loop through all drivers
            for (int i = 0; i < F1Info.MAX_AMOUNT_OF_CARS; i++)
            {
                bool validDriver;
                DriverData driverData = GameManager.F1Info.ReadCarData(i, out validDriver);

                //Skip the drivers that have no valid index -> junk data
                if (!validDriver)
                    continue;

                byte driverID = driverData.ParticipantData.driverID;
                byte carPosition = driverData.LapData.carPosition;

                //If this driver doesn't exist, it has just joined the session, recalculate everything!
                if (!_driverPosition.ContainsKey(driverID))
                    InitDrivers();

                //Drivers position has changed! Update!
                if (_driverPosition[driverID] != carPosition)
                {
                    int positionIndex = carPosition - 1; //Index in array is always one less than position

                    _driverTemplates[positionIndex].SetInitials(driverData.ParticipantData.driverInitial); //Set initals for that position

                    //Change color wether driver GAINED or LOST to this position -> compare old position with this one
                    _driverTemplates[positionIndex].UpdatePositionColor(_driverPosition[driverID], _movedUpColor, _movedDownColor);
                    _driverTemplates[positionIndex].SetTeamColor(driverData.ParticipantData.teamColor); //Set team color

                    //save this position to compare in future
                    _driverPosition[driverID] = carPosition;
                }
            }
        }

        /// <summary>
        /// A timing station that holds time and lap when leader passed
        /// </summary>
        struct DriverTimingData
        {
            public byte Lap { get; set; }
            public float Time { get; set; }
        }
    }
}
