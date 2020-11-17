using UnityEngine;

public class LobbyInfoPacket : Packet
{
    public LobbyInfoPacket(byte[] data) : base(data) { }

    public override void LoadBytes()
    {
        base.LoadBytes();
        Debug.Log("I AM LOBBY_INFO_PACKET!");
    }
}
