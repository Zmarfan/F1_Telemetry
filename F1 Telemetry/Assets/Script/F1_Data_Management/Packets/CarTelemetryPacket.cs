using System.Collections.Generic;
using System;

namespace F1_Data_Management
{
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

        protected override void LoadBytes()
        {
            base.LoadBytes();

            ByteManager manager = new ByteManager(Data, MOVE_PAST_HEADER_INDEX, "Car Telemetry Packet");
            AllCarTelemetryData = new CarTelemetryData[F1Info.MAX_AMOUNT_OF_CARS];

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
        }
    }

    /// <summary>
    /// Holds CarTelemetryData for one driver in the race.
    /// </summary>
    [System.Serializable]
    public struct CarTelemetryData
    {
        /// <summary>
        /// Current car speed in km/h
        /// </summary>
        public ushort speed;
        /// <summary>
        /// Amount of throttle (0.0 - 1.0)
        /// </summary>
        public float throttle;
        /// <summary>
        /// left(-1.0) - right(1.0)
        /// </summary>
        public float steer;
        /// <summary>
        /// Amount of brake (0.0 - 1.0)
        /// </summary>
        public float brake;
        /// <summary>
        /// Amount of clutch applied (0 to 100)
        /// </summary>
        public byte clutch;
        /// <summary>
        /// 0 = N & -1 = Reversed
        /// </summary>
        public sbyte gear;
        public ushort engineRPM;
        /// <summary>
        /// Is DRS currently activated
        /// </summary>
        public bool DRS;
        public byte revLightPercent;

        /// <summary>
        /// Brake temperature per tyre in celcius. Use Wheel class to index correct wheel.
        /// </summary>
        public ushort[] brakeTemperatures;
        /// <summary>
        /// Tyre surface temperature per tyre in celcius. Use Wheel class to index correct wheel.
        /// </summary>
        public byte[] tyreSurfaceTemperatures;
        /// <summary>
        /// Tyre inner temperature per tyre in celcius. Use Wheel class to index correct wheel.
        /// </summary>
        public byte[] tyreInnerTemperatures;
        /// <summary>
        /// Engine temperature in celcius
        /// </summary>
        public ushort engineTemperature;
        /// <summary>
        /// Tyre pressure per tyre in PSI. Use Wheel class to index correct wheel.
        /// </summary>
        public float[] tyrePressures;
        /// <summary>
        /// Surface type per tyre. Use Wheel class to index correct wheel.
        /// </summary>
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
}