using F1_Data_Management;

namespace F1_Unity
{
    public class DetailDeltaLeader : DetailDelta
    {
        protected override void UpdateActivatable()
        {
            bool status1 = false;
            bool status2 = false;
            DriverData d2Data = GameManager.F1Info.ReadSpectatingCarData(out status1);

            if (status1)
            {
                //Getting driverData of leader if spectating car is not leading
                if (d2Data.LapData.carPosition - 2 >= 0)
                {
                    DriverData d1Data = GameManager.DriverDataManager.GetDriverFromPosition(1, out status2);
                    SetData(d1Data, d2Data);
                }
                //It's the leader
                else
                {
                    //Read car behind spectator if he is leading
                    DriverData d1Data = GameManager.DriverDataManager.GetDriverFromPosition(d2Data.LapData.carPosition + 1, out status2);
                    SetData(d2Data, d1Data);
                }
            }

            if (status1 && status2)
                Show(true);
            else
                Show(false);
        }
    }
}