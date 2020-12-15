using F1_Data_Management;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;

namespace F1_Unity
{
    /// <summary>
    /// Display fastest lap information
    /// </summary>
    public class FastestLap : EventBase
    {
        [Header("Drop")]

        [SerializeField] Text _firstNameText;
        [SerializeField] Text _SecondNameText;
        [SerializeField] Text _oneNameText;       //Off by default
        [SerializeField] Text _time;
        [SerializeField] Image _teamImage;

        /// <summary>
        /// Init fastest lap with correct settings. time in seconds is converted to display format
        /// </summary>
        public override void Init(Packet packet)
        {
            base.Init(packet);

            //Cast to correct type
            FastestLapEventPacket fastestLapPacket = (FastestLapEventPacket)packet;

            DriverData data = GameManager.F1Info.ReadCarData(fastestLapPacket.VehicleIndex, out bool status);
            string fullName = string.Empty;
            Sprite teamSprite = GameManager.ParticipantManager.GetTeamSprite(Team.My_Team_Or_Unknown);

            //If data is valid (99.99 % of the time it is valid but hey for that 0.01 boi :3)
            if (status)
            {
                fullName = GameManager.ParticipantManager.GetNameFromNumber(data.RaceNumber);
                teamSprite = GameManager.ParticipantManager.GetTeamSprite(data.ParticipantData.team);
            }
            
            float time = fastestLapPacket.LapTime;

            InitTime(time);
            InitName(fullName);

            _teamImage.sprite = teamSprite;
        }

        /// <summary>
        /// Converts time in seconds to display format and sets it
        /// </summary>
        void InitTime(float timeInSeconds)
        {
            _time.text = F1Utility.GetDeltaString(timeInSeconds);
        }

        /// <summary>
        /// Breaks up and sets the name to be shown in fastest lap
        /// </summary>
        void InitName(string fullName)
        {
            string[] nameParts = fullName.Split();
            string firstName = nameParts[0];
            string lastName = string.Empty;

            //Create lastname if possible, if not -> single name only!
            if (nameParts.Length > 1)
            {
                for (int i = 1; i < nameParts.Length; i++)
                    lastName += nameParts[i];
                //Display first name then lastname
                _firstNameText.text = firstName;
                _SecondNameText.text = lastName.ToUpper();
            }
            //Display fullname as one in the middle
            else
            {
                _firstNameText.enabled = false;
                _SecondNameText.enabled = false;
                _oneNameText.enabled = true;
                _oneNameText.text = fullName.ToUpper();
            }
        }
    }
}
