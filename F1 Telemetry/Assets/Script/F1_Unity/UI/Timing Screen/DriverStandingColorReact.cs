using UnityEngine;
using F1_Data_Management;

namespace F1_Unity
{
    public class DriverStandingColorReact : MonoBehaviour
    {
        [SerializeField] UnityEngine.Color _fastestLapColor;
        [SerializeField] UnityEngine.Color _warningColor;
        [SerializeField] UnityEngine.Color _penaltyColor;
        [SerializeField] DriverTemplate[] _driverTemplates;

        private void OnEnable()
        {
            GameManager.F1Info.FastestLapEvent += FastestLapEvent;
            GameManager.F1Info.PenaltyEvent += PenaltyEvent;
        }

        private void OnDisable()
        {
            GameManager.F1Info.FastestLapEvent -= FastestLapEvent;     
            GameManager.F1Info.PenaltyEvent -= PenaltyEvent;     
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
            {
                for (int i = 0; i < _driverTemplates.Length; i++)
                    _driverTemplates[i].SetFastestLap(false);

                _driverTemplates[driverData.LapData.carPosition - 1].SetFastestLap(true);
                _driverTemplates[driverData.LapData.carPosition - 1].SetColor(_fastestLapColor);
            }
        }

        /// <summary>
        /// Called when a penalty event is sent
        /// </summary>
        public void PenaltyEvent(Packet packet)
        {
            PenaltyEventPacket penaltyPacket = (PenaltyEventPacket)packet;
            DriverData driverData = GameManager.F1Info.ReadCarData(penaltyPacket.VehicleIndex, out bool valid);

            if (valid)
            {
                InfringementType i = penaltyPacket.InfringementType;
                PenaltyType t = penaltyPacket.PenaltyType;

                //It's a warning for track limits
                if (penaltyPacket.PenaltyType == PenaltyType.Warning && (i == InfringementType.Corner_cutting_gained_time || i == InfringementType.Corner_cutting_overtake_multiple ||
                    i == InfringementType.Corner_cutting_overtake_single || i == InfringementType.Corner_cutting_ran_wide_gained_time_extreme || i == InfringementType.Corner_cutting_ran_wide_gained_time_minor ||
                    i == InfringementType.Corner_cutting_ran_wide_gained_time_significant || i == InfringementType.Lap_invalidated_corner_cutting))
                {
                    _driverTemplates[driverData.LapData.carPosition - 1].SetColor(_warningColor);
                }
                //It's a penalty
                else if (!(t == PenaltyType.Penalty_Reminder || t == PenaltyType.Warning || t == PenaltyType.This_And_Next_Lap_Invalidated || t == PenaltyType.This_And_Next_Lap_Invalidated_Without_Reason ||
                    t == PenaltyType.This_And_Previous_Lap_Invalidated || t == PenaltyType.This_And_Previous_Lap_Invalidated_Without_Reason || t == PenaltyType.This_Lap_Invalidated ||
                    t == PenaltyType.This_Lap_Invalidated_Without_Reason || t == PenaltyType.Retired))
                {
                    _driverTemplates[driverData.LapData.carPosition - 1].SetColor(_penaltyColor);
                }
            } 
        }
    }
}

