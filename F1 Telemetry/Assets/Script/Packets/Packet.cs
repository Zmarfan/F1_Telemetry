using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Base Class for packets, holds header variables shared by all packets and virtual functions for reading data in packet
/// </summary>
public class Packet
{
    public readonly int STATEMENT_TRUE = 1;
    protected static readonly int PACKET_ID_INDEX = 5;
    protected static readonly int MOVE_PAST_HEADER_INDEX = 24;

    public byte[] Data { get; protected set; }

    public PacketFormat PacketFormat { get; private set; }     //Game release year
    public byte GameMajorVersion { get; private set; }         //X.00
    public byte GameMinorVersion { get; private set; }         //1.XX
    public byte PacketVersion { get; private set; }            
    public byte PacketID { get; private set; }                 //What type of package is this? Identifier
    public ulong SessionUniqueID { get; private set; }         
    public float SessionTime { get; private set; }             //Total uptime of session
    public uint FrameID { get; private set; }                  // Identifier for the frame the data was retrieved on
    public byte PlayerCarIndex { get; private set; }           // Index of player's car in the array
    public byte SecondaryPlayerCarIndex { get; private set; }  //Index of secondary player's car in the array (splitscreen) -> 255 if no secondary player

    public Packet(byte[] data)
    {
        this.Data = data;
    }

    /// <summary>
    /// Returns packet-type of current packet
    /// </summary>
    public static PacketType GetPacketType(byte[] data)
    {
        ByteManager manager = new ByteManager(data);
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
