using System.Collections.Generic;
using System.Linq;

namespace F1_Data_Management
{

    /// <summary>
    /// Holds participant data to be reached from entire project. Entry point for outside scripts to gain access to F1 UDP data for all cars.
    /// </summary>
    public static class Participants
    {
        public const int MAX_AMOUNT_OF_CARS = 22;

        static DriverData[] _data = new DriverData[MAX_AMOUNT_OF_CARS];
        static bool _participantDataReady = false;
        static bool _motionDataReady = false;
        static bool _lapDataReady = false;
        static bool _telemetryDataReady = false;
        static bool _carStatusDataReady = false;
        static bool _carSetupDataReady = false;

        /// <summary>
        /// Amount of drivers actually competing -> Indexes can fall outside this value! Don't use for indexing! 0 if not in use
        /// </summary>
        public static int ActiveDrivers { get; private set; }

        /// <summary>
        /// Only read data when it's actually there
        /// </summary>
        public static bool ReadyToReadFrom { get { return _participantDataReady && _motionDataReady && _lapDataReady && _telemetryDataReady && _carStatusDataReady && _carSetupDataReady; } }

        #region Helpers

        /// <summary>
        /// Clear Data and don't allow to read the empty data.
        /// </summary>
        public static void Clear()
        {
            _data = new DriverData[MAX_AMOUNT_OF_CARS];
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
        static bool ContainsData(int index)
        {
            return !(_data[index].LapData.resultStatus == ResultStatus.Invalid || _data[index].LapData.resultStatus == ResultStatus.Inactive);
        }

        /// <summary>
        /// Is index within correct ranges to be able to read carData?
        /// </summary>
        static bool ValidIndex(int index)
        {
            return index >= 0 && index < MAX_AMOUNT_OF_CARS;
        }

        /// <summary>
        /// Used by member functions to check if data is ready to read from.
        /// </summary>
        static void CheckIfReadyToRead()
        {
            if (!ReadyToReadFrom)
                throw new System.Exception("Make sure data is ready to read from first! Check with ReadyToReadFrom");
        }

        #endregion

        #region Get functions

        /// <summary>
        /// Attempt to read data for vehicle. validData indicates if data returned is valid data.
        /// </summary>
        public static DriverData ReadCarData(int vehicleIndex, out bool validData)
        {
            CheckIfReadyToRead();
            if (!ValidIndex(vehicleIndex))
                throw new System.Exception("Make sure vehicleIndex is between values 0 and " + MAX_AMOUNT_OF_CARS);

            validData = ContainsData(vehicleIndex);
            return _data[vehicleIndex];
        }

        #endregion

        #region SetData

        /// <summary>
        /// Called every 5s from PacketManager to refresh data
        /// </summary>
        public static void SetParticipantsPacket(ParticipantsPacket data)
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
        public static void SetMotionPacket(MotionPacket data)
        {
            _motionDataReady = true;
            for (int i = 0; i < _data.Length; i++)
                _data[i].MotionData = data.AllCarMotionData[i];
        }

        /// <summary>
        /// Called often from PacketManager to refresh data
        ///<para>Only to be called from PackatManager to refresh data.</para>
        /// </summary>
        public static void SetLapData(LapDataPacket data)
        {
            _lapDataReady = true;
            for (int i = 0; i < _data.Length; i++)
                _data[i].LapData = data.LapData[i];
        }

        /// <summary>
        /// Called often from PacketManager to refresh data
        /// <para>Only to be called from PackatManager to refresh data.</para>
        /// </summary>
        public static void SetTelemetryData(CarTelemetryPacket data)
        {
            _telemetryDataReady = true;
            for (int i = 0; i < _data.Length; i++)
                _data[i].TelemetryData = data.AllCarTelemetryData[i];
        }

        /// <summary>
        /// Called often from PacketManager to refresh data
        /// <para>Only to be called from PackatManager to refresh data.</para>
        /// </summary>
        public static void SetCarStatusData(CarStatusPacket data)
        {
            _carStatusDataReady = true;
            for (int i = 0; i < _data.Length; i++)
                _data[i].StatusData = data.AllCarStatusData[i];
        }

        /// <summary>
        /// Called 2 times every second from PacketManager to refresh data
        /// <para>Only to be called from PackatManager to refresh data.</para>
        /// </summary>
        public static void SetCarSetupData(CarSetupPacket data)
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
    public struct DriverData
    {
        public ParticipantData ParticipantData { get; set; }
        public CarMotionData MotionData { get; set; }
        public LapData LapData { get; set; }
        public CarTelemetryData TelemetryData { get; set; }
        public CarStatusData StatusData { get; set; }
        public CarSetup CarSetup { get; set; }
    }
}