using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Base Class for packets, holds header variables shared by all packets and virtual functions for reading data in packet
/// </summary>
public class Packet
{
    protected readonly int PACKET_ID_INDEX = 5;
    protected readonly int MOVE_PAST_HEADER_INDEX = 24;

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

        //Copy data as to not mix upp references -> keep data secured
        for (int i = 0; i < data.Length; i++)
            copyData[i] = data[i];

        this.Data = copyData;
    }

    /// <summary>
    /// Returns packet-type of current packet
    /// </summary>
    public PacketType GetPacketType()
    {
        ByteManager manager = new ByteManager(Data);
        byte id = manager.GetByteAt(PACKET_ID_INDEX);
        return (PacketType)id;
    }

    /// <summary>
    /// Reads data and set variables, can be overriden by inherited classes (other packet types).
    /// Base packet class read Packet Header only
    /// </summary>
    public virtual void LoadBytes()
    {
        ByteManager manager = new ByteManager(Data);

        PacketFormat = GetPacketFormat(manager.GetBytes(sizeof(ushort)));
        GameMajorVersion = manager.GetByte();
        GameMinorVersion = manager.GetByte();
        PacketVersion = manager.GetByte();
        PacketID = manager.GetByte();
        SessionUniqueID = BitConverter.ToUInt64(manager.GetBytes(sizeof(long)), 0);
        SessionTime = BitConverter.ToSingle(manager.GetBytes(sizeof(float)), 0);
        FrameID = BitConverter.ToUInt32(manager.GetBytes(sizeof(uint)), 0);
        PlayerCarIndex = manager.GetByte();
        SecondaryPlayerCarIndex = manager.GetByte();
    }

    /// <summary>
    /// What game is this packet from? (Only using F1 2020 though)
    /// </summary>
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
