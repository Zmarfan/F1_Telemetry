using UnityEngine;
using F1_Data_Management;

namespace F1_Unity
{
    public class DriverStandingColorReact : MonoBehaviour
    {
        [SerializeField, Tooltip("Used in Q to always show fastest lap animation on P1")] bool _fastestLapOnLeader = false;
        [SerializeField] bool _fastestLap = true;
        [SerializeField] bool _penaltyWarning = true;
        [SerializeField] Color _fastestLapColor;
        [SerializeField] Color _warningColor;
        [SerializeField] Color _penaltyColor;
        [SerializeField] TimingScreenEntry[] _driverTemplates;

        private void OnEnable()
        {
            if (_fastestLap)
                GameManager.LapManager.FastestLapEvent += FastestLapEvent;
            if (_penaltyWarning)
                GameManager.F1Info.PenaltyEvent += PenaltyEvent;
        }

        private void OnDisable()
        {
            if (_fastestLap)
                GameManager.LapManager.FastestLapEvent -= FastestLapEvent;
            if (_penaltyWarning)
                GameManager.F1Info.PenaltyEvent -= PenaltyEvent;     
        }

        /// <summary>
        /// Called when fastest lap is set
        /// </summary>
        /// <param name="packet"></param>
        public void FastestLapEvent(DriverData driverData, float time)
        {
            _driverTemplates[_fastestLapOnLeader ? 0 : driverData.LapData.carPosition - 1].SetColor(_fastestLapColor);
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

