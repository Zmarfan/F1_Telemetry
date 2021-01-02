namespace F1_Data_Management
{
    /// <summary>
    /// This packet details Car status for all the cars in the race.
    /// Rate specified in menus (high frequency)
    /// </summary>
    public class CarStatusPacket : Packet
    {
        /// <summary>
        /// Will have 22 instances but only active cars will have valid car status data. Rest is junk values.
        /// </summary>
        public CarStatusData[] AllCarStatusData { get; private set; }

        public CarStatusPacket(byte[] data) : base(data) { }

        protected override void LoadBytes()
        {
            base.LoadBytes();

            ByteManager manager = new ByteManager(Data, MOVE_PAST_HEADER_INDEX, "Car Status Data Packet");
            AllCarStatusData = new CarStatusData[F1Info.MAX_AMOUNT_OF_CARS];

            for (int i = 0; i < AllCarStatusData.Length; i++)
            {
                AllCarStatusData[i].tractionControl = manager.GetEnumFromByte<TractionControlType>();
                AllCarStatusData[i].ABS = manager.GetBool();
                AllCarStatusData[i].fuelMix = manager.GetEnumFromByte<FuelMix>();
                AllCarStatusData[i].frontBrakeBias = manager.GetByte();
                AllCarStatusData[i].pitLimiterStatus = manager.GetBool();
                AllCarStatusData[i].fuelInTank = manager.GetFloat();
                AllCarStatusData[i].fuelCapacity = manager.GetFloat();
                AllCarStatusData[i].fuelRemainingLaps = manager.GetFloat();
                AllCarStatusData[i].maxRPM = manager.GetUnsignedShort();
                AllCarStatusData[i].idleRPM = manager.GetUnsignedShort();
                AllCarStatusData[i].maxGears = manager.GetByte();
                AllCarStatusData[i].DRSAllowed = manager.GetBool();

                AllCarStatusData[i].DRSActivationDistance = manager.GetUnsignedShort();
                AllCarStatusData[i].tyreWear = manager.GetBytes(Wheel.WHEEL_COUNT);
                AllCarStatusData[i].actualTyreCompound = manager.GetEnumFromByte<ActualTyreCompound>();
                AllCarStatusData[i].visualTyreCompound = manager.GetEnumFromByte<VisualTyreCompound>();
                AllCarStatusData[i].tyreAgeInLaps = manager.GetByte();
                AllCarStatusData[i].tyreDamage = manager.GetBytes(Wheel.WHEEL_COUNT);
                AllCarStatusData[i].frontLeftWingDamage = manager.GetByte();
                AllCarStatusData[i].frontRightWingDamage = manager.GetByte();
                AllCarStatusData[i].rearWingDamage = manager.GetByte();

                AllCarStatusData[i].DRSFault = manager.GetBool();
                AllCarStatusData[i].engineDamage = manager.GetByte();
                AllCarStatusData[i].gearBoxDamage = manager.GetByte();
                AllCarStatusData[i].vehicleFIAFlag = manager.GetEnumFromSignedByte<Flag>();
                AllCarStatusData[i].ERSStoreEnergy = manager.GetFloat();
                AllCarStatusData[i].ERSDeploymentMode = manager.GetEnumFromByte<ERSDeploymentMode>();
                AllCarStatusData[i].ERSHarvestedThisLapMGUK = manager.GetFloat();
                AllCarStatusData[i].ERSHarvestedThisLapMGUH = manager.GetFloat();
                AllCarStatusData[i].ERSDeployedThisLap = manager.GetFloat();
            }
        }
    }

    /// <summary>
    /// Holds CarStatusData for one driver in the race.
    /// Restricted variables are not accessable if Player hasn't allowed so.
    /// </summary>
    [System.Serializable]
    public struct CarStatusData
    {
        public TractionControlType tractionControl;
        /// <summary>
        /// True if using ABS
        /// </summary>
        public bool ABS;
        /// <summary>
        /// !Restricted!
        /// </summary>
        public FuelMix fuelMix;
        /// <summary>
        /// !Restricted! Percentage
        /// </summary>
        public byte frontBrakeBias;
        /// <summary>
        /// True if on.
        /// </summary>
        public bool pitLimiterStatus;
        /// <summary>
        /// !Restricted! Current fuel mass
        /// </summary>
        public float fuelInTank;
        /// <summary>
        /// !Restricted!
        /// </summary>
        public float fuelCapacity;
        /// <summary>
        /// !Restricted! Value on MFD.
        /// </summary>
        public float fuelRemainingLaps;
        /// <summary>
        /// Point of rev limiter
        /// </summary>
        public ushort maxRPM;
        public ushort idleRPM;
        public byte maxGears;
        public bool DRSAllowed;

        /// <summary>
        /// 0 => DRS not available, < 0 distance in metres
        /// </summary>
        public ushort DRSActivationDistance;
        /// <summary>
        /// !Restricted! Tyre wear percentage
        /// </summary>
        public byte[] tyreWear;
        /// <summary>
        /// Actual tyre currently fitted. (C1, C2, C3...)
        /// </summary>
        public ActualTyreCompound actualTyreCompound;
        /// <summary>
        /// The visual type of tyre currently fitted. (soft, medium, hard etc)
        /// </summary>
        public VisualTyreCompound visualTyreCompound;
        /// <summary>
        /// !Restricted!
        /// </summary>
        public byte tyreAgeInLaps;
        /// <summary>
        /// !Restricted! Tyre damage percentage
        /// </summary>
        public byte[] tyreDamage;
        /// <summary>
        /// !Restricted! Percentage.
        /// </summary>
        public byte frontLeftWingDamage;
        /// <summary>
        /// !Restricted! Percentage.
        /// </summary>
        public byte frontRightWingDamage;
        /// <summary>
        /// !Restricted! Percentage.
        /// </summary>
        public byte rearWingDamage;

        /// <summary>
        /// True if faulty.
        /// </summary>
        public bool DRSFault;
        public byte engineDamage;
        /// <summary>
        /// !Restricted! Percentage.
        /// </summary>
        public byte gearBoxDamage;
        /// <summary>
        /// !Restricted! Percentage.
        /// </summary>
        public Flag vehicleFIAFlag;
        /// <summary>
        /// !Restricted! Stored in Joules.
        /// </summary>
        public float ERSStoreEnergy;
        /// <summary>
        /// !Restricted!
        /// </summary>
        public ERSDeploymentMode ERSDeploymentMode;
        /// <summary>
        /// !Restricted! MGU-K is Motor Generator Unit - Kinetic -> energy through braking  | Joules
        /// </summary>
        public float ERSHarvestedThisLapMGUK;
        /// <summary>
        /// !Restricted! MGU-H is Motor Generator Unit - Heat -> energy through being | Joules
        /// </summary>
        public float ERSHarvestedThisLapMGUH;
        /// <summary>
        /// !Restricted! Joules.
        /// </summary>
        public float ERSDeployedThisLap;

        public static readonly float MAX_ERS_IN_JOULES = 4000000f;

        /// <summary>
        /// !Restricted!
        /// </summary>
        public int PercentageOfERSRemaining { get { return (int)(ERSStoreEnergy / MAX_ERS_IN_JOULES * 100); } }
        /// <summary>
        /// !Restricted!
        /// </summary>
        public int PercentageOfERSDeployedThisLap { get { return (int)(ERSDeployedThisLap / MAX_ERS_IN_JOULES * 100); } }
        /// <summary>
        /// !Restricted!
        /// </summary>
        public int PercentageOfERSHarvestedThisLap { get { return (int)((ERSHarvestedThisLapMGUK + ERSHarvestedThisLapMGUH) / MAX_ERS_IN_JOULES * 100); } }
        /// <summary>
        /// !Restricted!
        /// </summary>
        public int EffectivePercentageOFERSHarvestedThisLap { get { return PercentageOfERSHarvestedThisLap - PercentageOfERSDeployedThisLap; } }
    }
}