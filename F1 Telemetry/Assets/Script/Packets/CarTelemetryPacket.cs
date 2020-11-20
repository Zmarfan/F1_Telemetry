using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// This packet details telemetry for all the cars in the race.
/// Rate specified in menus (high frequency)
/// </summary>
public class CarTelemetryPacket : Packet
{
    /// <summary>
    /// Will have 22 instances but only active cars will have valid setups. Rest is junk values.
    /// </summary>
    public CarTelemetryData[] AllCarTelemetryData { get; private set; }
    public ButtonStatus ButtonStatus { get; private set; }                        //Bit flags specifying which buttons are being pressed
    public MFDPanelType MFDPanelType { get; private set; }                
    public MFDPanelType MFDPanelTypeSecondaryPlayer { get; private set; }
    public sbyte SuggestedGear { get; private set; }                       //0 -> no gear suggestion activated

    public CarTelemetryPacket(byte[] data) : base(data) { }

    public override void LoadBytes()
    {
        base.LoadBytes();

        ByteManager manager = new ByteManager(Data, MOVE_PAST_HEADER_INDEX, "Car Telemetry Packet");
        AllCarTelemetryData = new CarTelemetryData[MAX_AMOUNT_OF_CARS];

        for (int i = 0; i < AllCarTelemetryData.Length; i++)
        {
            AllCarTelemetryData[i].speed = manager.GetUnsignedShort();
            AllCarTelemetryData[i].throttle = manager.GetFloat();
            AllCarTelemetryData[i].steer = manager.GetFloat();
            AllCarTelemetryData[i].brake = manager.GetFloat();
            AllCarTelemetryData[i].clutch = manager.GetByte();
            AllCarTelemetryData[i].gear = manager.GetSignedByte();
            AllCarTelemetryData[i].engineRPM = manager.GetUnsignedShort();
            AllCarTelemetryData[i].DRS = manager.GetBool();
            AllCarTelemetryData[i].revLightPercent = manager.GetByte();
            AllCarTelemetryData[i].brakeTemperatures = manager.GetUnsignedShortArray(Wheel.WHEEL_COUNT);
            AllCarTelemetryData[i].tyreSurfaceTemperatures = manager.GetBytes(Wheel.WHEEL_COUNT);
            AllCarTelemetryData[i].tyreInnerTemperatures = manager.GetBytes(Wheel.WHEEL_COUNT);
            AllCarTelemetryData[i].engineTemperature = manager.GetUnsignedShort();
            AllCarTelemetryData[i].tyrePressures = manager.GetFloatArray(Wheel.WHEEL_COUNT);
            AllCarTelemetryData[i].surfaceTypes = manager.GetEnumArrayFromBytes<SurfaceType>(Wheel.WHEEL_COUNT);
        }

        ButtonStatus = new ButtonStatus(manager.GetUnsignedInt());
        MFDPanelType = manager.GetEnumFromByte<MFDPanelType>();
        MFDPanelTypeSecondaryPlayer = manager.GetEnumFromByte<MFDPanelType>();
        SuggestedGear = manager.GetSignedByte();

        Debug.Log(ButtonStatus.GetInput(ButtonInputTypes.A_Button));
    }
}

/// <summary>
/// Holds CarTelemetryData for one driver in the race.
/// </summary>
public struct CarTelemetryData
{
    public ushort speed;         //In km/h
    public float throttle;       //Amount of throttle (0.0 - 1.0)
    public float steer;          //left(-1.0) - right(1.0)
    public float brake;          //Amount of brake (0.0 - 1.0)
    public byte clutch;          //Amount of clutch applied (0 to 100)
    public sbyte gear;           //0 = N & -1 = Reversed
    public ushort engineRPM;
    public bool DRS;             //True if on
    public byte revLightPercent; //percentage

    public ushort[] brakeTemperatures; //Celcius
    public byte[] tyreSurfaceTemperatures; //Celcius
    public byte[] tyreInnerTemperatures; //Celcius
    public ushort engineTemperature; //Celcius
    public float[] tyrePressures; //PSI
    public SurfaceType[] surfaceTypes;
}

/// <summary>
/// Holds input from player
/// </summary>
public struct ButtonStatus
{
    //Always safe to access since it always has all enums 
    Dictionary<ButtonInputTypes, bool> inputDictionary;

    public ButtonStatus(uint data)
    {
        inputDictionary = new Dictionary<ButtonInputTypes, bool>();
        //Read data and set it correctly in dictionary
        Array allInputTypes = Enum.GetValues(typeof(ButtonInputTypes));
        for (int i = 0; i < allInputTypes.Length; i++)
            AddInputType(data, (ButtonInputTypes)allInputTypes.GetValue(i));
    }

    /// <summary>
    /// Adds input type in dictionary and sets it to true => held down or false => not held down 
    /// </summary>
    void AddInputType(uint data, ButtonInputTypes type)
    {
        //Each type has an integer value which matches AND of data for that input
        inputDictionary.Add(type, (data & (int)type) == (int)type);
    }

    /// <summary>
    /// Returns if specified button is currently held down
    /// </summary>
    public bool GetInput(ButtonInputTypes type)
    {
        return inputDictionary[type];
    }
}
