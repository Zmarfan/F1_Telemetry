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

        public override void LoadBytes()
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
    public struct CarStatusData
    {
        public TractionControlType tractionControl;
        public bool ABS;                                         //True if using ABS
        public FuelMix fuelMix;//Restricted                                             
        public byte frontBrakeBias;//Restricted                  //percentage
        public bool pitLimiterStatus;                            //True if on
        public float fuelInTank;//Restricted                     //Current fuel mass
        public float fuelCapacity;//Restricted                                          
        public float fuelRemainingLaps;//Restricted              //Value on MFD
        public ushort maxRPM;                                    //point of rev limiter
        public ushort idleRPM;
        public byte maxGears;
        public bool DRSAllowed;                                  //True if allowed

        public ushort DRSActivationDistance;                     //0 => DRS not available, < 0 distance in metres
        public byte[] tyreWear;//Restricted                      //Tyre wear percentage
        public ActualTyreCompound actualTyreCompound;
        public VisualTyreCompound visualTyreCompound;
        public byte tyreAgeInLaps;//Restricted                   
        public byte[] tyreDamage;//Restricted                    //Tyre damage percentage
        public byte frontLeftWingDamage;//Restricted             //Percentage
        public byte frontRightWingDamage;//Restricted            //Percentage
        public byte rearWingDamage;//Restricted                  //Percentage

        public bool DRSFault;                                    //True if faulty
        public byte engineDamage;//Restricted                    //Percentage
        public byte gearBoxDamage;//Restricted                   //Percentage
        public Flag vehicleFIAFlag;
        public float ERSStoreEnergy;//Restricted                 //Stored in Joules (Experiment)
        public ERSDeploymentMode ERSDeploymentMode;//Restricted
        public float ERSHarvestedThisLapMGUK;//Restricted        //MGU-K is Motor Generator Unit - Kinetic -> energy through braking  | Joules
        public float ERSHarvestedThisLapMGUH;//Restricted        //MGU-H is Motor Generator Unit - Heat -> energy through being | Joules
        public float ERSDeployedThisLap; //Restricted            //Joules

        public static readonly float MAX_ERS_IN_JOULES = 4000000f;

        //Restricted
        public int PercentageOfERSRemaining { get { return (int)(ERSStoreEnergy / MAX_ERS_IN_JOULES * 100); } }
        public int PercentageOfERSDeployedThisLap { get { return (int)(ERSDeployedThisLap / MAX_ERS_IN_JOULES * 100); } }
        public int PercentageOfERSHarvestedThisLap { get { return (int)((ERSHarvestedThisLapMGUK + ERSHarvestedThisLapMGUH) / MAX_ERS_IN_JOULES * 100); } }
        public int EffectivePercentageOFERSHarvestedThisLap { get { return PercentageOfERSHarvestedThisLap - PercentageOfERSDeployedThisLap; } }
    }
}