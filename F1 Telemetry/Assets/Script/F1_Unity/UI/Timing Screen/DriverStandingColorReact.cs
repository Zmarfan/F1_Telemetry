using UnityEngine;
using F1_Data_Management;

namespace F1_Unity
{
    public class DriverStandingColorReact : MonoBehaviour
    {
        [SerializeField] UnityEngine.Color _fastestLapColor;
        [SerializeField] UnityEngine.Color _warningColor;
        [SerializeField] DriverTemplate[] _driverTemplates;

        private void Start()
        {
            _driverTemplates[5].SetColor(_fastestLapColor);
        }

        private void OnEnable()
        {
            GameManager.F1Info.FastestLapEvent += FastestLapEvent;
        }

        private void OnDisable()
        {
            GameManager.F1Info.FastestLapEvent -= FastestLapEvent;     
        }

        /// <summary>
        /// Called when fastest lap is set
        /// </summary>
        /// <param name="packet"></param>
        public void FastestLapEvent(Packet packet)
        {
            FastestLapEventPacket fastestPacket = (FastestLapEventPacket)packet;
            DriverData driverData = GameManager.F1Info.ReadCarData(fastestPacket.VehicleIndex, out bool valid);
            if (valid)
                _driverTemplates[driverData.LapData.carPosition - 1].SetColor(_fastestLapColor);
        }

        /// <summary>
        /// Called when a penalty event is sent
        /// </summary>
        public void PenaltyEvent(Packet packet)
        {
            PenaltyEventPacket penaltyPacket = (PenaltyEventPacket)packet;
            if (penaltyPacket.PenaltyType == PenaltyType.Warning)
            {
                Debug.Log(penaltyPacket.InfringementType);
            }
            DriverData driverData = GameManager.F1Info.ReadCarData(penaltyPacket.VehicleIndex, out bool valid);
            if (valid)
                _driverTemplates[driverData.LapData.carPosition - 1].SetColor(_fastestLapColor);
        }
    }
}

