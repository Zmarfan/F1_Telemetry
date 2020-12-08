using F1_Data_Management;
using UnityEngine;
using UnityEngine.UI;

namespace F1_Unity
{
    public class RaceWinner : EventBase
    {
        [SerializeField] Text _driverText;
        [SerializeField] Text _teamText;
        [SerializeField] Image _teamImage;

        public override void Init(Packet packet)
        {
            base.Init(packet);
            RaceWinnerEventPacket winnerPacket = (RaceWinnerEventPacket)packet;

            DriverData data = GameManager.F1Info.ReadCarData(winnerPacket.VehicleIndex, out bool status);

            if (status)
            {
                _driverText.text = ParticipantManager.GetNameFromNumber(data.RaceNumber).Replace('_', ' ');
                _teamText.text = data.ParticipantData.team.ToString().Replace('_', ' ');
                _teamImage.color = TeamColor.GetColorByTeam(data.ParticipantData.team);
            }
        }
    }
}