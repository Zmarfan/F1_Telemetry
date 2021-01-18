using System.Collections.Generic;
using UnityEngine;
using F1_Data_Management;
using System.Linq;

namespace F1_Unity
{
    public class QTimingScreen : TimingScreenBase
    {
        /// <summary>
        /// Maps driver IDs to position on track. Used to compare old position to current, called on when data is available
        /// </summary>
        protected override void InitDrivers()
        {
            _initValues = false;
            SetMode(_timeScreenState);

            _driverPosition = new Dictionary<byte, int>();

            Session sessionData = GameManager.F1Info.ReadSession(out bool status);

            for (int i = 0; i < F1Info.MAX_AMOUNT_OF_CARS; i++)
            {
                DriverData driverData = GameManager.F1Info.ReadCarData(i, out bool validDriver);
                ResultStatus resultStatus = ResultStatus.Inactive;

                //Init everyone with gaining a position on start!
                //Flash green then to white
                //Only add valid drivers to the grid
                if (validDriver && status)
                {
                    _driverPosition.Add(driverData.ID, driverData.LapData.carPosition + 1);
                    _driverEntries[i].SetTimingState(DriverTimeState.No_Time_Q);
                    _driverEntries[i].SetTiming(); 
                    //Sets driverData if it's usage is needed
                    _driverEntries[GetEntryIndex(driverData)].SetDriverData(driverData);

                    //If the car is already retired -> set it so
                    resultStatus = driverData.LapData.resultStatus;
                    if (resultStatus == ResultStatus.Retired || resultStatus == ResultStatus.Disqualified)
                        _driverEntries[i].Out(resultStatus);
                    else
                        _driverEntries[i].NotOut();
                    //Init leader
                    if (driverData.LapData.carPosition == 1)
                        _leaderVehicleIndex = driverData.VehicleIndex;
                }

                //Only enable so many positions in time standing as there are active drivers
                //If they DNF/DSQ later they will only gray out, not be removed
                _driverEntries[i].SetActive(!(resultStatus == ResultStatus.Inactive || resultStatus == ResultStatus.Invalid));
            }
        }

        /// <summary>
        /// Updates positions in standing and checks stability, also updates driver stats
        /// </summary>
        protected override void MainUpdate()
        {
            //Will be valid -> checked earlier
            Session sessionData = GameManager.F1Info.ReadSession(out bool status);
            DriverData leaderData = GameManager.F1Info.ReadCarData(_leaderVehicleIndex, out bool status2);

            //Loop through all drivers
            for (int i = 0; i < F1Info.MAX_AMOUNT_OF_CARS; i++)
            {
                DriverData driverData = GameManager.F1Info.ReadCarData(i, out bool validDriver);
                //Skip the drivers that have no valid index -> junk data
                if (!validDriver)
                    continue;
                DoDriver(driverData, leaderData, sessionData);
            }

            CalculateDriverIntervals();
        }

        /// <summary>
        /// Sets all aspect of timing for specific driver
        /// </summary>
        protected override void DoDriver(DriverData driverData, DriverData leaderData, Session sessionData)
        {
            int index = GetEntryIndex(driverData);
            //Set specific driver info to entry
            _driverEntries[index].SetDriverData(driverData);
            _driverEntries[index].SetFastestLap(driverData);
            _driverEntries[index].SetSpectator(driverData);

            //Update stats for driver
            _driverEntries[index].UpdateStats(driverData, _availableStatsState[_timingStatsStateIndex]);

            Positioning(driverData, sessionData, index);
            //Need to read leaderData again in case this driver overtook him
            leaderData = GameManager.F1Info.ReadCarData(_leaderVehicleIndex, out bool status3);

            bool inPit = IsDriverInPit(driverData, index);
            _driverEntries[index].SetInPit(inPit);
            UpdateDriverTimingToLeader(leaderData, sessionData, driverData, index);
        }

        /// <summary>
        /// Change driver position and exchange values between entries -> update leader values if leader changed
        /// </summary>
        /// <param name="carPosition">New car position for driver</param>
        /// <param name="driverData">Data for driver that has changed position</param>
        /// <param name="sessionData">Session data</param>
        /// <param name="index">entry index for driver</param>
        protected override void ChangeDriverPosition(byte carPosition, DriverData driverData, Session sessionData, int index)
        {
            //Update indexes for leader
            if (carPosition == 1)
                _leaderVehicleIndex = driverData.VehicleIndex;

            _driverEntries[index].SetInitials(GameManager.ParticipantManager.GetDriverInitials(driverData.RaceNumber)); //Set initals for that position
            _driverEntries[index].SetTeamColor(GameManager.F1Utility.GetColorByTeam(driverData.ParticipantData.team)); //Set team color

            //if the car is retired, set it to out
            ResultStatus resultStatus = driverData.LapData.resultStatus;
            if (resultStatus == ResultStatus.Retired || resultStatus == ResultStatus.Disqualified)
                _driverEntries[index].Out(resultStatus);
            else
            {
                //If it was previously out make it not
                if (_driverEntries[index].OutOfSession)
                    _driverEntries[index].NotOut();

                //Change color wether driver GAINED or LOST to this position -> compare old position with this one
                _driverEntries[index].UpdatePositionColor(_driverPosition[driverData.ID], _movedUpColor, _movedDownColor);
            }

            //save this position to compare in future
            _driverPosition[driverData.ID] = carPosition;
        }


        /// <summary>
        /// Sets the time text for a driver depending on distance to leader in fastest lap difference.
        /// </summary>
        void UpdateDriverTimingToLeader(DriverData leaderData, Session sessionData, DriverData driverData, int index)
        {
            float fastestLap = driverData.LapData.bestLapTime;
            float leaderFastestLap = leaderData.LapData.bestLapTime;
            float deltaToLeader = fastestLap - leaderFastestLap;

            _driverEntries[index].SetDeltaToLeader(deltaToLeader);

            //Leader
            if (leaderData.VehicleIndex == driverData.VehicleIndex)
            {
                //It is leader so delta to itself is 0
                _driverEntries[index].SetDeltaToLeader(0);
                _driverEntries[index].SetTimingState(DriverTimeState.Leader_Q);
            }
            //Hasn't done a time yet
            else if (driverData.LapData.bestLapTime == 0)
                _driverEntries[index].SetTimingState(DriverTimeState.No_Time_Q);
            //If nothing else -> Show delta
            else
                _driverEntries[index].SetTimingState(DriverTimeState.Delta);

            if (driverData.LapData.driverStatus == DriverStatus.Out_Lap)
                _driverEntries[index].SetTimingState(DriverTimeState.Out_Lap_Q);             
        }

        /// <summary>
        /// Checks if driver is currently in pit and sets it so in entry
        /// </summary>
        override protected bool IsDriverInPit(DriverData driverData, int index)
        {
            if (driverData.LapData.pitStatus == PitStatus.Pitting || driverData.LapData.pitStatus == PitStatus.In_Pit_Area)
            {
                _driverEntries[index].SetTimingState(DriverTimeState.Pit_Q);
                return true;
            }
            return false;
        }
    }
}