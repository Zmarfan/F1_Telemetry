namespace F1_Data_Management
{
    /// <summary>
    /// Different types of packets sent by F1 2020
    /// </summary>
    public enum PacketType
    {
        MOTION,
        SESSION,
        LAP_DATA,
        EVENT,
        PARTICIPANTS,
        CAR_SETUPS,
        CAR_TELEMETRY,
        CAR_STATUS,
        FINAL_CLASSIFICATION,
        LOBBY_INFO
    }
}