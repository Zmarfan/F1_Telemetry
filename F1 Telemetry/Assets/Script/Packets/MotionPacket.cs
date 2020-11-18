using UnityEngine;

public class MotionPacket : Packet
{
    public CarMotionData[] AllCarMotionData { get; private set; }

    public MotionPacket(byte[] data) : base(data) { }

    public override void LoadBytes()
    {
        base.LoadBytes();
        //CONTINUE HERE!
    }
}

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
    public float worldRIghtDirectionZ;

    public float gForceLateral;      //G-force from left to right TEST
    public float gForceLongitudinal; //G-force from back-front    TEST
    public float gForceVertical;     //G-force from down-up       TEST
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
