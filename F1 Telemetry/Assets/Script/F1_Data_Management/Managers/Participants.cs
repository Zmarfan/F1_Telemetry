using System.Collections.Generic;
using System.Linq;

namespace F1_Data_Management
{

    /// <summary>
    /// Holds participant data to be reached from entire project. Entry point for outside scripts to gain access to F1 UDP data for all cars.
    /// </summary>
    public class Participants
    {
        DriverData[] _data = new DriverData[F1Info.MAX_AMOUNT_OF_CARS];
        bool _participantDataReady = false;
        bool _motionDataReady = false;
        bool _lapDataReady = false;
        bool _telemetryDataReady = false;
        bool _carStatusDataReady = false;
        bool _carSetupDataReady = false;

        /// <summary>
        /// Amount of drivers actually competing -> Indexes can fall outside this value! Don't use for indexing! 0 if not in use
        /// </summary>
        public int ActiveDrivers { get; private set; }

        /// <summary>
        /// Only read data when it's actually there
        /// </summary>
        public bool ReadyToReadFrom { get { return _participantDataReady && _motionDataReady && _lapDataReady && _telemetryDataReady && _carStatusDataReady && _carSetupDataReady; } }

        #region Helpers

        /// <summary>
        /// Clear Data and don't allow to read the empty data.
        /// </summary>
        public void Clear()
        {
            _data = new DriverData[F1Info.MAX_AMOUNT_OF_CARS];
            ActiveDrivers = 0;
            _participantDataReady = false;
            _motionDataReady = false;
            _lapDataReady = false;
            _telemetryDataReady = false;
            _carStatusDataReady = false;
            _carSetupDataReady = false;
        }

        /// <summary>
        /// Returns true wether or not the index of a car actually contains valid data. False means it's junk data and should be disregarded.
        /// </summary>
        bool ContainsData(int index)
        {
            return !(_data[index].LapData.resultStatus == ResultStatus.Invalid || _data[index].LapData.resultStatus == ResultStatus.Inactive);
        }

        /// <summary>
        /// Is index within correct ranges to be able to read carData?
        /// </summary>
        bool ValidIndex(int index)
        {
            return index >= 0 && index < F1Info.MAX_AMOUNT_OF_CARS;
        }
        #endregion

        #region Get functions

        /// <summary>
        /// Attempt to read data for vehicle. validData indicates if data returned is valid data.
        /// </summary>
        /// <param name="vehicleIndex">Index of the car which data you want to get. Must be within 0 - 22.</param>
        /// <param name="validData">Indicates if returned data is valid data. Unvalid means either -> vehicle doesn't exist or data not yet set</param>
        public DriverData ReadCarData(int vehicleIndex, out bool validData)
        {
            if (!ValidIndex(vehicleIndex))
                throw new System.Exception("Make sure vehicleIndex is between values 0 and " + F1Info.MAX_AMOUNT_OF_CARS);

            validData = ReadyToReadFrom ? ContainsData(vehicleIndex) : false;
            return _data[vehicleIndex];
        }

        #endregion

        #region SetData

        /// <summary>
        /// Called every 5s from PacketManager to refresh data
        /// </summary>
        public void SetParticipantsPacket(ParticipantsPacket data)
        {
            _participantDataReady = true;
            ActiveDrivers = data.NumberOfActiveCars;
            for (int i = 0; i < _data.Length; i++)
                _data[i].ParticipantData = data.AllParticipantData[i];
        }

        /// <summary>
        /// Called 2 times per second from PacketManager to refresh data.
        /// <para>Only to be called from PackatManager to refresh data.</para>
        /// </summary>
        public void SetMotionPacket(MotionPacket data)
        {
            _motionDataReady = true;
            for (int i = 0; i < _data.Length; i++)
                _data[i].MotionData = data.AllCarMotionData[i];
        }

        /// <summary>
        /// Called often from PacketManager to refresh data
        ///<para>Only to be called from PackatManager to refresh data.</para>
        /// </summary>
        public void SetLapData(LapDataPacket data)
        {
            _lapDataReady = true;
            for (int i = 0; i < _data.Length; i++)
                _data[i].LapData = data.LapData[i];
        }

        /// <summary>
        /// Called often from PacketManager to refresh data
        /// <para>Only to be called from PackatManager to refresh data.</para>
        /// </summary>
        public void SetTelemetryData(CarTelemetryPacket data)
        {
            _telemetryDataReady = true;
            for (int i = 0; i < _data.Length; i++)
                _data[i].TelemetryData = data.AllCarTelemetryData[i];
        }

        /// <summary>
        /// Called often from PacketManager to refresh data
        /// <para>Only to be called from PackatManager to refresh data.</para>
        /// </summary>
        public void SetCarStatusData(CarStatusPacket data)
        {
            _carStatusDataReady = true;
            for (int i = 0; i < _data.Length; i++)
                _data[i].StatusData = data.AllCarStatusData[i];
        }

        /// <summary>
        /// Called 2 times every second from PacketManager to refresh data
        /// <para>Only to be called from PackatManager to refresh data.</para>
        /// </summary>
        public void SetCarSetupData(CarSetupPacket data)
        {
            _carSetupDataReady = true;
            for (int i = 0; i < _data.Length; i++)
                _data[i].CarSetup = data.AllCarSetups[i];
        }

        #endregion
    }

    /// <summary>
    /// Holds all data for one driver.
    /// </summary>
    [System.Serializable]
    public struct DriverData
    {
        /// <summary>
        /// Index for this drivers data when reading.
        /// </summary>
        public int VehicleIndex { get { return ParticipantData.vehicleIndex; } }
        /// <summary>
        /// Id unique for this driver in the session (better to identify with than index)
        /// </summary>
        public byte ID { get { return ParticipantData.driverID; } }
        /// <summary>
        /// Race number for this driver -> good for identifying multiplayer names
        /// </summary>
        public byte RaceNumber { get { return ParticipantData.raceNumber; } }
        public ParticipantData ParticipantData { get; set; }
        public CarMotionData MotionData { get; set; }
        public LapData LapData { get; set; }
        public CarTelemetryData TelemetryData { get; set; }
        public CarStatusData StatusData { get; set; }
        public CarSetup CarSetup { get; set; }
    }
}