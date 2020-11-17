using UnityEngine;

public class ParticipantsPacket : Packet
{
    public ParticipantsPacket(byte[] data) : base(data) { }

    public override void LoadBytes()
    {
        base.LoadBytes();
        Debug.Log("I AM PARTICIPANTS_PACKET!");
    }
}