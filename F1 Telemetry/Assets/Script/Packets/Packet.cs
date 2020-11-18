using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Base Class for packets, holds header variables shared by all packets and virtual functions for reading data in packet
/// </summary>
public class Packet
{
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
        ByteManager manager = new ByteManager(data, 0, "Get packet type");
        byte id = manager.GetByteAt(PACKET_ID_INDEX);
        return (PacketType)id;
    }

    /// <summary>
    /// Reads data and set variables, can be overriden by inherited classes (other packet types).
    /// Base packet class read Packet Header only
    /// </summary>
    public virtual void LoadBytes()
    {
        ByteManager manager = new ByteManager(Data, 0, "Header packet");

        PacketFormat = (PacketFormat)manager.GetUnsignedShort(); //Will return 2020 which will be turned to F1_2020 enum
        GameMajorVersion = manager.GetByte();
        GameMinorVersion = manager.GetByte();
        PacketVersion = manager.GetByte();
        PacketID = manager.GetByte();
        SessionUniqueID = manager.GetUnsignedLong();
        SessionTime = manager.GetFloat();
        FrameID = manager.GetUnsignedInt();
        PlayerCarIndex = manager.GetByte();
        SecondaryPlayerCarIndex = manager.GetByte();
    }
}
