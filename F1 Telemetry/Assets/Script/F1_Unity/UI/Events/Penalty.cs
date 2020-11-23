using F1_Data_Management;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace F1_Unity
{
    public class Penalty : EventBase
    {
        [Header("Drop")]

        //Holds types to react to, use last if unsure -> testing required
        [SerializeField] string _defaultPenaltyString;
        [SerializeField] List<PenaltyString> _penaltyToString;
        [SerializeField] string _defaultOffenceString;
        [SerializeField] List<OffenceString> _offenceToString;

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
            DriverData data = Participants.ReadCarData(penaltyPacket.VehicleIndex, out status);
            string fullName = string.Empty;

            //If data is valid (99.99 % of the time it is valid but hey for that 0.01 boi :3)
            if (status)
                fullName = RaceNames.GetNameFromNumber(data.ParticipantData.raceNumber);

            InitName(fullName);
            InitPenalty(penaltyPacket.PenaltyType);
            InitOffence(penaltyPacket.InfringementType);
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
            List<PenaltyString> matches = _penaltyToString.Where(item => item._penaltyType.Contains(type)).ToList();
            //If there exist match (should only be one -> use first found)
            if (matches.Count > 0)
                _penaltyTypeText.text = matches[0]._showName;
            else
            {
                //Set to standard text and return warning
                Debug.LogWarning("There exist no support for this penalty type in: " + this.ToString());
                _penaltyTypeText.text = _defaultPenaltyString;
            }
        }

        /// <summary>
        /// Sets offence type text based on offence type
        /// </summary>
        void InitOffence(InfringementType type)
        {
            List<OffenceString> matches = _offenceToString.Where(item => item._offenceType.Contains(type)).ToList();
            //If there exist match (should only be one -> use first found)
            if (matches.Count > 0)
                _infringementTypeText.text = matches[0]._showName;
            else
            {
                //Set to standard text and return warning
                Debug.LogWarning("There exist no support for this penalty type in: " + this.ToString());
                _infringementTypeText.text = _defaultOffenceString;
            }
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
