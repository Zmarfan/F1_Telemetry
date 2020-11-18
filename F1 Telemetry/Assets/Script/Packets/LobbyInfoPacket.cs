using UnityEngine;

/// <summary>
/// This packet details the players currently in a multiplayer lobby. 
/// 2 packets sent every second when in lobby
/// </summary>
public class LobbyInfoPacket : Packet
{
    public byte NumberOfPlayers { get; private set; }
    public LobbyInfoData[] AllLobbyInfoData { get; private set; }

    public LobbyInfoPacket(byte[] data) : base(data) { }

    public override void LoadBytes()
    {
        base.LoadBytes();

        ByteManager manager = new ByteManager(Data, MOVE_PAST_HEADER_INDEX, "Lobby Info Packet");

        NumberOfPlayers = manager.GetByte();
        AllLobbyInfoData = new LobbyInfoData[NumberOfPlayers];

        for (int i = 0; i < AllLobbyInfoData.Length; i++)
        {
            AllLobbyInfoData[i].AIControlled = manager.GetBool();
            AllLobbyInfoData[i].team = (Team)manager.GetByte();
            AllLobbyInfoData[i].nationality = (Nationality)manager.GetByte();
            AllLobbyInfoData[i].name = manager.GetString(LobbyInfoData.AMOUNT_OF_CHARS_IN_NAME);
            AllLobbyInfoData[i].readyStatus = (ReadyStatus)manager.GetByte();
        }
    }
}

public struct LobbyInfoData
{
    public bool AIControlled; //True if car is controlled by AI
    public Team team;
    public Nationality nationality;
    public string name;
    public ReadyStatus readyStatus;

    public static readonly int AMOUNT_OF_CHARS_IN_NAME = 48; //Amount of bytes to make up name in LobbyInfoData / Packet
}
