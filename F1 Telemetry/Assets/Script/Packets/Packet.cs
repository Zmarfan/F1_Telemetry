using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Base Class 
/// </summary>
public class Packet
{
    public byte[] Data { get; protected set; }

    public PacketFormat PacketFormat { get; protected set; }     //Game release year
    public byte GameMajorVersion { get; protected set; }         //X.00
    public byte GameMinorVersion { get; protected set; }         //1.XX
    public byte PacketVersion { get; protected set; }            
    public byte PacketID { get; protected set; }                 //What type of package is this? Identifier
    public ulong SessionUniqueID { get; protected set; }         
    public float SessionTime { get; protected set; }             //Total uptime of session
    public uint FrameID { get; protected set; }                  // Identifier for the frame the data was retrieved on
    public byte PlayerCarIndex { get; protected set; }           // Index of player's car in the array
    public byte SecondaryPlayerCarIndex { get; protected set; }  //Index of secondary player's car in the array (splitscreen) -> 255 if no secondary player

    public Packet(byte[] data)
    {
        byte[] copyData = new byte[data.Length];

        for (int i = 0; i < data.Length; i++)
            copyData[i] = data[i];

        this.Data = copyData;
    }

    public int GetPacketIndex()
    {
        ByteManager manager = new ByteManager(Data);
        byte id = manager.GetByteAt(5);
        return id;
    }

    public virtual void LoadBytes()
    {
        ByteManager manager = new ByteManager(Data);

        PacketFormat = GetPacketFormat(manager.GetBytes(2));
        GameMajorVersion = manager.GetByte();
        GameMinorVersion = manager.GetByte();
        PacketVersion = manager.GetByte();
        PacketID = manager.GetByte();
        SessionUniqueID = BitConverter.ToUInt64(manager.GetBytes(8), 0);
        SessionTime = BitConverter.ToSingle(manager.GetBytes(4), 0);
        FrameID = BitConverter.ToUInt32(manager.GetBytes(4), 0);
        PlayerCarIndex = manager.GetByte();
        SecondaryPlayerCarIndex = manager.GetByte();
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
