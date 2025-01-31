﻿using UnityEngine;
using F1_Data_Management;
using System.Collections.Generic;
using System;
using System.IO;

namespace F1_Unity
{
    public delegate void FastestSectorDelegate(DriverData driverData, LapState sector, float time);
    public delegate void FastestLapDelegate(DriverData driverData, float time);

    /// <summary>
    /// Singleton class that hold all laps and their sector times for every lap of every driver.
    /// </summary>
    public class LapManager : MonoBehaviour
    {
        /// <summary>
        /// Invoked whenever a new fastest sector is set.
        /// </summary>
        public event FastestSectorDelegate FastestSectorEvent;
        /// <summary>
        /// Invoked whenever a new fastest lap is set.
        /// </summary>
        public event FastestLapDelegate FastestLapEvent;

        /// <summary>
        /// Index of the driver with fastest lap
        /// </summary>
        public float FastestLapTime { get; private set; }

        //Index represent vehicle index. Stores all lap data for every driver
        StoredDriverData[] _storedDriverData = new StoredDriverData[F1Info.MAX_AMOUNT_OF_CARS];

        float _currentFastestSector1;
        float _currentFastestSector2;
        float _currentFastestSector3;

        /// <summary>
        /// Float maxvalue if not done any, in seconds
        /// </summary>
        public float CurrentFastestSector1 { get { return _currentFastestSector1; } private set { _currentFastestSector1 = value; } }
        /// <summary>
        /// Float maxvalue if not done any, in seconds
        /// </summary>
        public float CurrentFastestSector2 { get { return _currentFastestSector2; } private set { _currentFastestSector2 = value; } }
        /// <summary>
        /// Float maxvalue if not done any, in seconds
        /// </summary>
        public float CurrentFastestSector3 { get { return _currentFastestSector3; } private set { _currentFastestSector3 = value; } }


        private void Awake()
        {
            ResetManager();
        }

        public void ResetManager()
        {
            FastestLapTime = float.MaxValue;

            _currentFastestSector1 = float.MaxValue;
            _currentFastestSector2 = float.MaxValue;
            _currentFastestSector3 = float.MaxValue;

            for (int i = 0; i < _storedDriverData.Length; i++)
                _storedDriverData[i] = new StoredDriverData();
        }

        private void OnEnable()
        {
            GameManager.F1Info.PenaltyEvent += HandlePenaltyEvent;
        }

        private void OnDisable()
        {
            GameManager.F1Info.PenaltyEvent -= HandlePenaltyEvent;           
        }

        /// <summary>
        /// Stores stop and go penalties for a specific driver
        /// </summary>
        void HandlePenaltyEvent(Packet packet)
        {
            PenaltyEventPacket penaltyPacket = (PenaltyEventPacket)packet;
            if (penaltyPacket.PenaltyType == PenaltyType.Stop_Go)
                _storedDriverData[penaltyPacket.VehicleIndex].AddStopGoPenalty(penaltyPacket.Time);
            else if (penaltyPacket.PenaltyType == PenaltyType.Drive_Through)
                _storedDriverData[penaltyPacket.VehicleIndex].GotDriveThrough();
        }

        /// <summary>
        /// Used to count pit stop a driver has made. Increment that drivers pitstop counter by one. 
        /// </summary>
        public void AddPitStop(int vehicleIndex)
        {
            _storedDriverData[vehicleIndex].IncrementPitCounter();
        }

        /// <summary>
        /// Returns how many times a driver has pitted.
        /// </summary>
        public byte TimesPitted(int vehicleIndex)
        {
            CheckVehicleIndex(vehicleIndex);
            return _storedDriverData[vehicleIndex].TimesPitted;
        }

        /// <summary>
        /// Returns how many stop go penalties driver has in seconds
        /// </summary>
        /// <param name="vehicleIndex">Which driver?</param>
        /// <returns>Amount of stop go penalties in seconds</returns>
        public byte AmountOfStopGoPenalties(int vehicleIndex)
        {
            CheckVehicleIndex(vehicleIndex);
            return _storedDriverData[vehicleIndex].StopGoPenalties;
        }

        public bool HasDriveThrough(int vehicleIndex)
        {
            CheckVehicleIndex(vehicleIndex);
            return _storedDriverData[vehicleIndex].HasDriveThrough;
        }

        /// <summary>
        /// Throws exception if vehicle index is out of range
        /// </summary>
        void CheckVehicleIndex(int vehicleIndex)
        {
            if (vehicleIndex < 0 || vehicleIndex >= F1Info.MAX_AMOUNT_OF_CARS)
                throw new Exception("Vehicle index falls out of range! 0-22! Index: " + vehicleIndex);
        }

        /// <summary>
        /// Gets StoredLapData for specific driver and lap. Make sure lap number is valid. Can't access future or negative laps.
        /// </summary>
        /// <param name="vehicleIndex">Vehicle index for driver whose lap you want.</param>
        /// <param name="lapNumber">What lap for that driver?</param>
        /// <param name="status">Indicates whether the received data is valid or not. Depending on when program was launched data may be junk.</param>
        public StoredLapData ReadDriverLapData(int vehicleIndex, byte lapNumber, out bool status)
        {
            //Invalid vehicle index
            if (vehicleIndex < 0 || vehicleIndex >= F1Info.MAX_AMOUNT_OF_CARS)
                throw new Exception("Vehicle index falls out of range! 0-22! Index: " + vehicleIndex);
            //Invalid lap number
            if (lapNumber <= 0)
                throw new Exception("Lap number must be positive");
            //Hasn't initilized yet -> return junk data
            if (lapNumber > _storedDriverData[vehicleIndex].CurrentLap)
            {
                status = false;
                return new StoredLapData();
            }

            //Driver either doesn't exist or a lap for that driver has not been recorded if it's lapstate is unknown
            status = _storedDriverData[vehicleIndex].LapState != LapState.Unknown;
            return _storedDriverData[vehicleIndex].GetStoredLapData(lapNumber);
        }

        private void Update()
        {
            if (GameManager.F1Info.ReadyToReadFrom)
                UpdateLapData();

            //DEBUGGING ONLY
            //if (Input.GetKeyDown(KeyCode.B))
            //    WriteFile();
            //DEBUGGING ONLY
        }

        //DEBUGGING ONLY
        void WriteFile()
        {
            try
            {
                Debug.Log("RUN");
                StreamWriter sw = new StreamWriter("C:\\Users\\Filip Petersson\\Documents\\GitHub\\F1_Telemetry\\F1 Telemetry\\Assets\\Debug\\LapTimes.txt");
                for (int i = 0; i < _storedDriverData.Length; i++)
                {
                    DriverData driverData = GameManager.F1Info.ReadCarData(i, out bool status);
                    if (status)
                        sw.WriteLine("Driver: " + GameManager.ParticipantManager.GetNameFromNumber(driverData.RaceNumber));
                    else
                        sw.WriteLine("empty index");

                    for (int j = 1; j <= _storedDriverData[i].CurrentLap; j++)
                    {
                        StoredLapData data = _storedDriverData[i].GetStoredLapData(j);
                        sw.Write("Lap: " + j.ToString());
                        sw.Write(", Sector 1: " + TimeSpan.FromMilliseconds(data.sector1).TotalSeconds.ToString());
                        sw.Write(", Sector 2: " + TimeSpan.FromMilliseconds(data.sector2).TotalSeconds.ToString());
                        sw.Write(", Sector 3: " + TimeSpan.FromMilliseconds(data.sector3).TotalSeconds.ToString());
                        sw.Write(", Lap Time: " + data.lapTime.ToString());
                        sw.WriteLine(", Tyre: " + data.tyreCompound.ToString());
                    }

                    sw.WriteLine();
                }
                sw.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
        }
        //DEBUGGING ONLY

        /// <summary>
        /// Updates all drivers stored lap data
        /// </summary>
        void UpdateLapData()
        {
            for (int i = 0; i < F1Info.MAX_AMOUNT_OF_CARS; i++)
            {
                DriverData driverData = GameManager.F1Info.ReadCarData(i, out bool status);
                if (status)
                {
                    //Also updates current fastest lap if it's faster than latest fastest lap
                    if (driverData.LapData.bestLapTime < FastestLapTime && driverData.LapData.bestLapTime != 0)
                    {
                        FastestLapTime = driverData.LapData.bestLapTime;
                        FastestLapEvent?.Invoke(driverData, FastestLapTime);
                    }

                    LapState sector = driverData.LapData.currentSector;
                    LapState storedSector = _storedDriverData[i].LapState;
                    //Means a lap is already recording
                    if (storedSector != LapState.Unknown)
                    {
                        //The driver just crossed the finish line to complete a lap
                        if (sector == LapState.Sector_1 && storedSector == LapState.Sector_2)
                        {
                            _storedDriverData[i].FinishLap(driverData);
                            CheckForFastestSector(driverData, LapState.Sector_3, driverData.LapData.bestOverallSector3Time, ref _currentFastestSector3);
                        }
                        //The Car just completed sector 1 -> Can add sector 1
                        else if (sector == LapState.Sector_2 && storedSector == LapState.Sector_3)
                        {
                            _storedDriverData[i].AddSector1(driverData.LapData);
                            CheckForFastestSector(driverData, LapState.Sector_1, driverData.LapData.bestOverallSector1Time, ref _currentFastestSector1);
                        }
                        //The car just completed sector 2 -> Can add sector 2
                        else if (sector == LapState.Sector_3 && storedSector == LapState.Sector_1)
                        {
                            _storedDriverData[i].AddSector2(driverData.LapData);
                            CheckForFastestSector(driverData, LapState.Sector_2, driverData.LapData.bestOverallSector2Time, ref _currentFastestSector2);
                        }
                    }
                    //Car has completed first sector and can start recording all laps now! 
                    //Must be when entering sector2 since then it's 100 % the values will be correct
                    else if (sector == LapState.Sector_2)
                        _storedDriverData[i].AddSector1(driverData.LapData);
                }
            }
        }

        /// <summary>
        /// Checks if the drivers best sector x time is the best of the session so far and invoke fastestSector event in that case.
        /// </summary>
        /// <param name="driverData">DriverData for the driver being checked</param>
        /// <param name="sector">What sector is this?</param>
        /// <param name="driversBest">What time is this driver's best sector x time</param>
        /// <param name="currentBest">Stored best sector x time in millieseconds</param>
        void CheckForFastestSector(DriverData driverData, LapState sector, ushort driversBest, ref float currentBest)
        {
            //On the first lap they haven't set a best yet -> return in that case
            if (driversBest == 0)
                return;

            TimeSpan span = TimeSpan.FromMilliseconds(driversBest);
            float driversBestInSeconds = (float)span.TotalSeconds;
            if (driversBestInSeconds < currentBest)
            {
                currentBest = driversBestInSeconds;
                FastestSectorEvent?.Invoke(driverData, sector, driversBestInSeconds);
            }
        }
    }

    [System.Serializable]
    public class StoredDriverData
    {
        List<StoredLapData> _lapDataList = new List<StoredLapData>();
        public byte TimesPitted { get; private set; } = 0;
        /// <summary>
        /// Amount of stop go penalties in seconds
        /// </summary>
        public byte  StopGoPenalties { get; private set; } = 0;
        public bool HasDriveThrough { get; private set; } = false;
        public int CurrentLap { get { return _lapDataList.Count; } }
        public LapState LapState { get; private set; } = LapState.Unknown;

        /// <summary>
        /// Get StoredLapData for a specific lap.
        /// </summary>
        public StoredLapData GetStoredLapData(int lapNumber)
        {
            //Check if lapNumber exist
            if (lapNumber > 0 && lapNumber <= CurrentLap)
                return _lapDataList[lapNumber - 1];
            else
                throw new Exception("Trying to access a lap not yet in the system! Current Lap in system:" + CurrentLap + ", access: " + lapNumber);
        }

        /// <summary>
        /// The player has now pitted, increment pit counter by one.
        /// </summary>
        public void IncrementPitCounter()
        {
            TimesPitted++;
        }

        /// <summary>
        /// Adds penalty to stop go penalty total count in seconds
        /// </summary>
        /// <param name="addValue">added penalty in seconds</param>
        public void AddStopGoPenalty(byte addValue)
        {
            StopGoPenalties += addValue;
        }

        /// <summary>
        /// The driver just received a drive through penalty
        /// </summary>
        public void GotDriveThrough()
        {
            HasDriveThrough = true;
        }

        /// <summary>
        /// Creates new StoredLapData instance and adds in queue with Sector1 time
        /// </summary>
        public void AddSector1(LapData data)
        {
            //If program is started mid race it will catch up to current lap number -> this data will be junk data and marked as such
            for (int i = _lapDataList.Count; i < data.currentLapNumber - 1; i++)
            {
                StoredLapData fillData = new StoredLapData() { lapState = LapState.Unknown };
                _lapDataList.Add(fillData);
            }

            //Add new lapData
            StoredLapData addData = new StoredLapData
            {
                sector1 = (float)TimeSpan.FromMilliseconds(data.sector1Time).TotalSeconds,
                lapState = LapState.Sector_1
            };

            LapState = LapState.Sector_1;
            _lapDataList.Add(addData);
        }

        /// <summary>
        /// Sets sector2 data in the last instance in List -> current lap
        /// </summary>
        public void AddSector2(LapData data)
        {
            StoredLapData lapData = _lapDataList[_lapDataList.Count - 1];
            lapData.sector2 = (float)TimeSpan.FromMilliseconds(data.sector2Time).TotalSeconds;
            lapData.lapState = LapState.Sector_2;
            LapState = LapState.Sector_2;
            _lapDataList[_lapDataList.Count - 1] = lapData;
        }

        /// <summary>
        /// Called when a new lap is started -> calculate sector3 and set total lap time and tyre compound
        /// </summary>
        public void FinishLap(DriverData data)
        {
            //Calculate sector3 and set lap time
            StoredLapData lapData = _lapDataList[_lapDataList.Count - 1];
            float lapTime = data.LapData.lastLapTime;

            lapData.sector3 = lapTime - lapData.sector1 - lapData.sector2;
            lapData.lapTime = lapTime;

            //What tyre was the lap made on?
            lapData.tyreCompound = data.StatusData.visualTyreCompound;
            lapData.lapState = LapState.Sector_3;
            LapState = LapState.Sector_3;
            _lapDataList[_lapDataList.Count - 1] = lapData;
        }
    }

    /// <summary>
    /// Holds data for one lap
    /// </summary>
    [System.Serializable]
    public struct StoredLapData
    {
        /// <summary>
        /// Time in seconds
        /// </summary>
        public float sector1;
        /// <summary>
        /// Time in seconds
        /// </summary>
        public float sector2;
        /// <summary>
        /// Time in seconds
        /// </summary>
        public float sector3;
        /// <summary>
        /// Total lap time in seconds
        /// </summary>
        public float lapTime;
        /// <summary>
        /// Indicates which values are valid depending on how far along lap car is.
        /// <para>Sector_1 means only sector_1 contains valid data</para>
        /// <para>Sector_2 means sector_1 and sector_2 contains valid data</para>
        /// <para>Sector_3 means all data is valid</para>
        /// <para>Unknown means entire data is junk and should be treated as such</para>
        /// </summary>
        public LapState lapState;
        /// <summary>
        /// What tyre was the lap completed on.
        /// </summary>
        public VisualTyreCompound tyreCompound;
    }
}