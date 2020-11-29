using F1_Data_Management;
using UnityEngine;
using UnityEngine.UI;

namespace F1_Unity
{
    public class SpeedTrap : EventBase
    {
        [SerializeField, Range(0, 50)] int _startShowingLap = 3;
        [SerializeField, Range(1, 4)] int _amountOfDecimals = 1;
        [SerializeField] Text _driverText;
        [SerializeField] Text _speedText;

        bool _tooEarly = false;

        public override void Init(Packet packet)
        {
            base.Init(packet);

            SpeedTrapEventPacket speedPacket = (SpeedTrapEventPacket)packet;

            DriverData driverData = GameManager.F1Info.ReadCarData(speedPacket.VehicleIndex, out bool status);

            //Early in the race it gets beaten all the time, no need to react to it until later
            if (status && driverData.LapData.currentLapNumber >= _startShowingLap)
            {
                _driverText.text = ParticipantManager.GetNameFromNumber(driverData.RaceNumber).ToUpper();
                float speed = speedPacket.Speed;
                int point = (int)(speed - (int)speed) * (int)Mathf.Pow(10, _amountOfDecimals);
                _speedText.text = speed + "." + point;
            }
            else
                _tooEarly = true;
        }

        public override void Begin()
        {
            if (_tooEarly)
                End();
            else
                base.Begin();
        }
    }
}