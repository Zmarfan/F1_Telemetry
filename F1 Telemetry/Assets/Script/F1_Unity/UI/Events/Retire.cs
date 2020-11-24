using F1_Data_Management;
using UnityEngine;
using UnityEngine.UI;

namespace F1_Unity
{
    public class Retire : EventBase
    {
        [SerializeField] Text _message; 

        public override void Init(Packet packet)
        {
            base.Init(packet);
            RetirementEventPacket retirePacket = (RetirementEventPacket)packet;
            bool status;
            DriverData data = GameManager.F1Info.ReadCarData(retirePacket.VehicleIndex, out status);
            //99.99 % certain it's valid but safety concern
            if (status)
                _message.text = RaceNames.GetNameFromNumber(data.ParticipantData.raceNumber).ToUpper() + " RETIRED";
        }
    }
}

