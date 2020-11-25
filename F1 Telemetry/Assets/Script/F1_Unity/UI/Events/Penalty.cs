using F1_Data_Management;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Text;

namespace F1_Unity
{
    public class Penalty : EventBase
    {
        [Header("Drop")]

        [SerializeField] Text _penaltyTypeText;
        [SerializeField] Text _infringementTypeText;
        [SerializeField] Text _firstNameText;
        [SerializeField] Text _lastNameText;
        [SerializeField] Text _oneNameText;
        [SerializeField] Text _penaltyAmountText;
        [SerializeField] Transform _secondsHolder;

        public override void Init(Packet packet)
        {
            base.Init(packet);

            //Cast to correct type
            PenaltyEventPacket penaltyPacket = (PenaltyEventPacket)packet;

            bool status;
            DriverData data = GameManager.F1Info.ReadCarData(penaltyPacket.VehicleIndex, out status);
            string fullName = string.Empty;

            //If data is valid (99.99 % of the time it is valid but hey for that 0.01 boi :3)
            if (status)
                fullName = RaceNames.GetNameFromNumber(data.RaceNumber);

            InitName(fullName);
            InitPenalty(penaltyPacket.PenaltyType);
            InitOffence(penaltyPacket.InfringementType);
            InitAmount(penaltyPacket.Time);
        }

        /// <summary>
        /// Breaks up and sets the name to be shown in Penalty Event
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
                _lastNameText.text = lastName.ToUpper();
            }
            //Display fullname as one in the middle
            else
            {
                _firstNameText.enabled = false;
                _lastNameText.enabled = false;
                _oneNameText.enabled = true;
                _oneNameText.text = fullName.ToUpper();
            }
        }

        /// <summary>
        /// Sets penalty type text based on penalty type
        /// </summary>
        void InitPenalty(PenaltyType type)
        {
            string[] words = type.ToString().Split(new char[] { '_' });
            //Up to this index should be first row
            int middleIndex = words.Length > 1 ? words.Length / 2 : words.Length;
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < middleIndex; i++)
            {
                builder.Append(words[i].ToUpper());
                builder.Append(' ');
            }
            builder.Append('\n');
            for (int i = middleIndex; i < words.Length; i++)
            {
                builder.Append(words[i].ToUpper());
                builder.Append(' ');
            }
            
            _penaltyTypeText.text = builder.ToString();
        }

        /// <summary>
        /// Sets offence type text based on offence type
        /// </summary>
        void InitOffence(InfringementType type)
        {
            string text = type.ToString().Replace('_', ' ').ToUpper();
            _infringementTypeText.text = text;
        }

        /// <summary>
        /// Sets the amount of seconds received if any. Remove that part if zero seconds added.
        /// </summary>
        void InitAmount(byte seconds)
        {
            if (seconds == byte.MaxValue)
                _secondsHolder.gameObject.SetActive(false);
            else
                _penaltyAmountText.text = "+" + seconds;
        }
    }

    /// <summary>
    /// Holds string correlation to penaltyType.
    /// </summary>
    [System.Serializable]
    public struct PenaltyString
    {
        public PenaltyType[] _penaltyType;
        public string _showName;
    }

    /// <summary>
    /// Holds string correlation to offenceType.
    /// </summary>
    [System.Serializable]
    public struct OffenceString
    {
        public InfringementType[] _offenceType;
        public string _showName;
    }
}
