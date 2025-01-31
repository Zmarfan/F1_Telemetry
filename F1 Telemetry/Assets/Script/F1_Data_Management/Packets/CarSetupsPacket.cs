﻿namespace F1_Data_Management
{
    /// <summary>
    /// This packet details car setups for AI and player (blank values for multiplayer cars).
    /// 2 packets every second.
    /// </summary>
    public class CarSetupPacket : Packet
    {
        /// <summary>
        /// Will have 22 instances but only active cars will have valid setups. Rest is junk values.
        /// </summary>
        public CarSetup[] AllCarSetups { get; private set; }

        public CarSetupPacket(byte[] data) : base(data) { }

        protected override void LoadBytes()
        {
            base.LoadBytes();

            ByteManager manager = new ByteManager(Data, MOVE_PAST_HEADER_INDEX, "Car Setup Packet");
            AllCarSetups = new CarSetup[F1Info.MAX_AMOUNT_OF_CARS];

            for (int i = 0; i < AllCarSetups.Length; i++)
            {
                AllCarSetups[i].frontWing = manager.GetByte();
                AllCarSetups[i].rearWing = manager.GetByte();
                AllCarSetups[i].onThrottle = manager.GetByte();
                AllCarSetups[i].offThrottle = manager.GetByte();
                AllCarSetups[i].frontCamber = manager.GetFloat();
                AllCarSetups[i].rearCamber = manager.GetFloat();
                AllCarSetups[i].frontToe = manager.GetFloat();
                AllCarSetups[i].rearToe = manager.GetFloat();
                AllCarSetups[i].frontSuspension = manager.GetByte();
                AllCarSetups[i].rearSuspension = manager.GetByte();
                AllCarSetups[i].frontAntiRollBar = manager.GetByte();
                AllCarSetups[i].rearAntiRollBar = manager.GetByte();
                AllCarSetups[i].frontSuspensionHeight = manager.GetByte();
                AllCarSetups[i].rearSuspensionHeight = manager.GetByte();
                AllCarSetups[i].brakePressure = manager.GetByte();
                AllCarSetups[i].brakeBias = manager.GetByte();
                AllCarSetups[i].rearLeftTyrePressure = manager.GetFloat();
                AllCarSetups[i].rearRightTyrePressure = manager.GetFloat();
                AllCarSetups[i].frontLeftTyrePressure = manager.GetFloat();
                AllCarSetups[i].frontRightTyrePressure = manager.GetFloat();
                AllCarSetups[i].ballast = manager.GetByte();
                AllCarSetups[i].fuelLoad = manager.GetFloat();
            }
        }
    }

    /// <summary>
    /// Holds CarSetup for one driver in the race.
    /// Blank values for multiplayer cars.
    /// </summary>
    [System.Serializable]
    public struct CarSetup
    {
        public byte frontWing;
        public byte rearWing;
        /// <summary>
        /// Differential adjustment on throttle (percentage)
        /// </summary>
        public byte onThrottle;
        /// <summary>
        /// Differential adjustment off throttle (percentage)
        /// </summary>
        public byte offThrottle;

        /// <summary>
        /// Angle (Suspension Geomentry)
        /// </summary>
        public float frontCamber;
        /// <summary>
        /// Angle (Suspension Geomentry)
        /// </summary>
        public float rearCamber;
        /// <summary>
        /// Angle (Suspension Geomentry)
        /// </summary>
        public float frontToe;
        /// <summary>
        /// Angle (Suspension Geomentry)
        /// </summary>
        public float rearToe;

        public byte frontSuspension;
        public byte rearSuspension;
        public byte frontAntiRollBar;
        public byte rearAntiRollBar;
        public byte frontSuspensionHeight;
        public byte rearSuspensionHeight;
        /// <summary>
        /// Percentage
        /// </summary>
        public byte brakePressure;
        /// <summary>
        /// Percentage
        /// </summary>
        public byte brakeBias;

        /// <summary>
        /// PSI
        /// </summary>
        public float rearLeftTyrePressure;
        /// <summary>
        /// PSI
        /// </summary>
        public float rearRightTyrePressure;
        /// <summary>
        /// PSI
        /// </summary>
        public float frontLeftTyrePressure;
        /// <summary>
        /// PSI
        /// </summary>
        public float frontRightTyrePressure;

        public byte ballast;
        public float fuelLoad;
    }
}