using System;

namespace F1_Data_Management
{
    /// <summary>
    /// Holds lobby info data for all drivers read from Lobby Info packets.
    /// </summary>
    [System.Serializable]
    public class LobbyInfoManager
    {
        /// <summary>
        /// Limit for when stored data is considered timed out. In seconds
        /// </summary>
        static readonly float LOBBY_INFO_TIME_OUT_LIMIT = 2f;

        /// <summary>
        /// Indicates if data exist
        /// </summary>
        public bool ReadyToReadFrom { get { return _lobbyInfoPacket != null; } }
        /// <summary>
        /// Latest packet received
        /// </summary>
        LobbyInfoPacket _lobbyInfoPacket;

        /// <summary>
        /// Returns how many players that currently are in lobby
        /// </summary>
        /// <param name="currentTime">Time when calling this getter. Compared with stored data to check for validity</param>
        /// <param name="status">indicates if read data is valid</param>
        /// <returns>Numbers of drivers in lobby</returns>
        public byte AmountOfDriversInLobby(float currentTime, out bool status)
        {
            //If data exist and is updated it is valid data
            status = ReadyToReadFrom && Math.Abs(currentTime - _lobbyInfoPacket.SessionTime) <= LOBBY_INFO_TIME_OUT_LIMIT;
            return _lobbyInfoPacket.NumberOfPlayers;
        }

        /// <summary>
        /// Returns lobby info data for specific driver
        /// </summary>
        /// <param name="currentTime">The current time when calling this getter. Compare time of data with this to check for validity</param>
        /// <param name="vehicleIndex">Index for driver</param>
        /// <returns>Lobby info data -> nationality etc</returns>
        public LobbyInfoData GetDriverLobbyInfoData(int vehicleIndex, float currentTime, out bool status)
        {
            if (!ValidIndex(vehicleIndex))
                throw new System.Exception("Make sure vehicleIndex is between values 0 and " + F1Info.MAX_AMOUNT_OF_CARS);
            //If data exist and is updated it is valid data
            status = ReadyToReadFrom && Math.Abs(currentTime - _lobbyInfoPacket.SessionTime) <= LOBBY_INFO_TIME_OUT_LIMIT;
            if (ReadyToReadFrom)
                return _lobbyInfoPacket.AllLobbyInfoData[vehicleIndex];
            //Return junk data if there is no data to read
            else
                return new LobbyInfoData();
        }

        /// <summary>
        /// Clears out saved data.
        /// </summary>
        public void Clear()
        {
            _lobbyInfoPacket = null;
        }

        /// <summary>
        /// Copy data from packet to interface for user to be able to read safely.
        /// </summary>
        public void UpdateLobbyInfoData(LobbyInfoPacket lobbyInfoPacket)
        {
            _lobbyInfoPacket = lobbyInfoPacket;
        }

        /// <summary>
        /// Is index within correct ranges to be able to read carData?
        /// </summary>
        bool ValidIndex(int index)
        {
            return index >= 0 && index < F1Info.MAX_AMOUNT_OF_CARS;
        }
    }
}