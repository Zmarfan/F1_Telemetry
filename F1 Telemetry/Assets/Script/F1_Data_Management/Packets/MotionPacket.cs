namespace F1_Data_Management
{
    /// <summary>
    /// The motion packet gives physics data for all the cars being driven. There is additional data for the car being driven
    /// Rate specified in menus (high frequency)
    /// </summary>
    public class MotionPacket : Packet
    {
        public CarMotionData[] AllCarMotionData { get; private set; }

        //Extra: Player car ONLY data
        //To reach correct wheel use Wheel.Index (4 wheels per float[])
        public float[] SuspensionPosition { get; private set; }
        public float[] SuspensionVelocity { get; private set; }
        public float[] SuspensionAcceleration { get; private set; }
        public float[] WheelSpeed { get; private set; }
        public float[] WheelSlip { get; private set; }

        public float LocalVelocityX { get; private set; }
        public float LocalVelocityY { get; private set; }
        public float LocalVelocityZ { get; private set; }
        public float AngularVelocityX { get; private set; }
        public float AngularVelocityY { get; private set; }
        public float AngularVelocityZ { get; private set; }
        public float AngularAccelerationX { get; private set; }
        public float AngularAccelerationY { get; private set; }
        public float AngularAccelerationZ { get; private set; }
        public float FrontWheelsAngle { get; private set; } //Positive -> turn right, Negative -> turn left

        public MotionPacket(byte[] data) : base(data) { }

        public override void LoadBytes()
        {
            base.LoadBytes();

            ByteManager manager = new ByteManager(Data, MOVE_PAST_HEADER_INDEX, "Motion Packet");

            AllCarMotionData = new CarMotionData[F1Info.MAX_AMOUNT_OF_CARS];
            //This will loop and assign as if there was 22 cars in the race!
            //If there are less cars than 22, these instances will be filled with junk values!
            for (int i = 0; i < F1Info.MAX_AMOUNT_OF_CARS; i++)
            {
                AllCarMotionData[i].worldPositionX = manager.GetFloat();
                AllCarMotionData[i].worldPositionY = manager.GetFloat();
                AllCarMotionData[i].worldPositionZ = manager.GetFloat();
                AllCarMotionData[i].worldVelocityX = manager.GetFloat();
                AllCarMotionData[i].worldVelocityY = manager.GetFloat();
                AllCarMotionData[i].worldVelocityZ = manager.GetFloat();

                AllCarMotionData[i].worldForwardDirectionX = CarMotionData.ConvertDataToNormalized(manager.GetShort());
                AllCarMotionData[i].worldForwardDirectionY = CarMotionData.ConvertDataToNormalized(manager.GetShort());
                AllCarMotionData[i].worldForwardDirectionZ = CarMotionData.ConvertDataToNormalized(manager.GetShort());
                AllCarMotionData[i].worldRightDirectionX = CarMotionData.ConvertDataToNormalized(manager.GetShort());
                AllCarMotionData[i].worldRightDirectionY = CarMotionData.ConvertDataToNormalized(manager.GetShort());
                AllCarMotionData[i].worldRightDirectionZ = CarMotionData.ConvertDataToNormalized(manager.GetShort());

                AllCarMotionData[i].gForceLateral = manager.GetFloat();
                AllCarMotionData[i].gForceLongitudinal = manager.GetFloat();
                AllCarMotionData[i].gForceVertical = manager.GetFloat();
                AllCarMotionData[i].yaw = manager.GetFloat();
                AllCarMotionData[i].pitch = manager.GetFloat();
                AllCarMotionData[i].roll = manager.GetFloat();
            }

            //Extra Player Car ONLY data
            //Wheel stuff
            SuspensionPosition = manager.GetFloatArray(Wheel.WHEEL_COUNT);
            SuspensionVelocity = manager.GetFloatArray(Wheel.WHEEL_COUNT);
            SuspensionAcceleration = manager.GetFloatArray(Wheel.WHEEL_COUNT);
            WheelSpeed = manager.GetFloatArray(Wheel.WHEEL_COUNT);
            WheelSlip = manager.GetFloatArray(Wheel.WHEEL_COUNT);

            LocalVelocityX = manager.GetFloat();
            LocalVelocityY = manager.GetFloat();
            LocalVelocityZ = manager.GetFloat();
            AngularVelocityX = manager.GetFloat();
            AngularVelocityY = manager.GetFloat();
            AngularVelocityY = manager.GetFloat();
            AngularAccelerationX = manager.GetFloat();
            AngularAccelerationY = manager.GetFloat();
            AngularAccelerationZ = manager.GetFloat();

            FrontWheelsAngle = manager.GetFloat();
        }
    }

    /// <summary>
    /// Holds CarMotionData for one driver in the race
    /// </summary>
    public struct CarMotionData
    {
        public float worldPositionX;
        public float worldPositionY;
        public float worldPositionZ;
        public float worldVelocityX;
        public float worldVelocityY;
        public float worldVelocityZ;

        public float worldForwardDirectionX;
        public float worldForwardDirectionY;
        public float worldForwardDirectionZ;
        public float worldRightDirectionX;
        public float worldRightDirectionY;
        public float worldRightDirectionZ;

        /// <summary>
        /// G-force negative when turning right, positive when turning left
        /// </summary>
        public float gForceLateral;
        /// <summary>
        /// G-force negative when braking, positive when accelerating
        /// </summary>
        public float gForceLongitudinal;
        /// <summary>
        /// G-force negative when lift up, positive when pushed down (not entirely sure)
        /// </summary>
        public float gForceVertical;
        public float yaw;
        public float pitch;
        public float roll;

        /// <summary>
        /// Used to convert data value to actual value for normalized direction components.
        /// </summary>
        public static float ConvertDataToNormalized(short dataValue)
        {
            return dataValue / CONVERSION_VALUE;
        }

        private static readonly float CONVERSION_VALUE = 32767.0f;
    }
}