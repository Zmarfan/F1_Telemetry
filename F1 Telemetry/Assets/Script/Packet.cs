using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Base Class 
/// </summary>
public class Packet
{
    protected byte[] _data;

    public PacketFormat PacketFormat { get; protected set; }
    public byte GameMajorVersion { get; protected set; } 
    public byte GameMinorVersion { get; protected set; }

    public Packet(byte[] data)
    {
        byte[] copyData = new byte[data.Length];

        for (int i = 0; i < data.Length; i++)
            copyData[i] = data[i];

        this._data = copyData;
    }

    public virtual void LoadBytes()
    {
        ByteManager manager = new ByteManager(_data);

        PacketFormat = GetPacketFormat(manager.GetBytes(2));
        GameMajorVersion = manager.GetByte();
        GameMinorVersion = manager.GetByte();
        Debug.Log(GameMajorVersion + "." + GameMinorVersion);
    }

    PacketFormat GetPacketFormat(byte[] data)
    {
        ushort packetFormat = BitConverter.ToUInt16(data, 0);

        switch (packetFormat)
        {
            case (2017): { return PacketFormat.F1_2017; }
            case (2018): { return PacketFormat.F1_2018; }
            case (2019): { return PacketFormat.F1_2019; }
            case (2020): { return PacketFormat.F1_2020; }
            default: return PacketFormat.UNKNOWN;
        }
    }
}
