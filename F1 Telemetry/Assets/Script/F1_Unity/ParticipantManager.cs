﻿using System.Collections.Generic;
using UnityEngine;
using F1_Data_Management;

namespace F1_Unity
{
    /// <summary>
    /// Holds all raceNumber to raceDriver correlation. Add multiplayer names here to available numbers.
    /// </summary>
    public class ParticipantManager : MonoBehaviour
    {
        [Header("Settings")]

        [SerializeField] char[] _splitters = new char[] { ' ', '_', '-' };
        [SerializeField] byte _initialLength = 3;

        [Header("Defaults")]

        [SerializeField] string _defaultDriverName = "Driver #";
        [SerializeField] Sprite _defaultPortrait;
        [SerializeField] Sprite _defaultTeamSprite;
        [SerializeField] Sprite _defaultCarSprite;

        [Header("Lists")]

        [SerializeField] NumberNameStruct[] _numberNameList;
        [SerializeField] NumberSpriteStruct[] _numberPortraitList;
        [SerializeField] TeamSpriteStruct[] _teamSpriteList;
        [SerializeField] TeamSpriteStruct[] _carSpriteList;

        static ParticipantManager _singleton;
        static Dictionary<byte, string> _namesByRaceNumber = new Dictionary<byte, string>();
        static Dictionary<byte, Sprite> _portraitByRaceNumber = new Dictionary<byte, Sprite>();
        static Dictionary<Team, Sprite> _teamSpriteByTeam = new Dictionary<Team, Sprite>();
        static Dictionary<Team, Sprite> _carSpriteByTeam = new Dictionary<Team, Sprite>();

        private void Awake()
        {
            if (_singleton == null)
                Init();
            else
                Destroy(this.gameObject);
        }

        /// <summary>
        /// Make a dictionary of list data for easy access
        /// </summary>
        void Init()
        {
            _singleton = this;
            for (int i = 0; i < _numberNameList.Length; i++)
                _namesByRaceNumber.Add(_numberNameList[i].raceNumber, _numberNameList[i].name);

            for (int i = 0; i < _numberPortraitList.Length; i++)
                _portraitByRaceNumber.Add(_numberPortraitList[i].raceNumber, _numberPortraitList[i].sprite);

            for (int i = 0; i < _teamSpriteList.Length; i++)
                _teamSpriteByTeam.Add(_teamSpriteList[i].team, _teamSpriteList[i].sprite);

            for (int i = 0; i < _carSpriteList.Length; i++)
                _carSpriteByTeam.Add(_carSpriteList[i].team, _carSpriteList[i].sprite);
        }

        /// <summary>
        /// Converts raceNumber to race driver name, returns "Driver #raceNumber" if not in system yet
        /// </summary>
        public static string GetNameFromNumber(byte raceNumber)
        {
            if (_namesByRaceNumber.ContainsKey(raceNumber))
                return _namesByRaceNumber[raceNumber];
            else
                return _singleton._defaultDriverName + raceNumber.ToString();
        }

        /// <summary>
        /// Converts raceNumber to race driver portrait, returns default portrait if not in system yet
        /// </summary>
        public static Sprite GetPortraitFromNumber(byte raceNumber)
        {
            if (_portraitByRaceNumber.ContainsKey(raceNumber))
                return _portraitByRaceNumber[raceNumber];
            else
                return _singleton._defaultPortrait;
        }

        /// <summary>
        /// Converts Team enum to team sprite, returns default sprite if not in system yet
        /// </summary>
        public static Sprite GetTeamSprite(Team team)
        {
            if (_teamSpriteByTeam.ContainsKey(team))
                return _teamSpriteByTeam[team];
            else
                return _singleton._defaultTeamSprite;
        }

        /// <summary>
        /// Converts Team enum to car sprite, returns default sprite if not in system yet
        /// </summary>
        public static Sprite GetCarSprite(Team team)
        {
            if (_carSpriteByTeam.ContainsKey(team))
                return _carSpriteByTeam[team];
            else
                return _singleton._defaultCarSprite;
        }

        /// <summary>
        /// <para> Returns 3 first letters in second name. Dashes/Underscores are treated as spaces. </para>
        /// If only one name -> first 3 letters in that.
        /// </summary>
        public static string GetDriverInitials(byte raceNumber)
        {
            string fullName = GetNameFromNumber(raceNumber);
            string[] words = fullName.Split(_singleton._splitters);

            //Get second word (surname) if available, otherwise take first word (firstName / Username for multiplayer)
            string initials = words.Length > 1 ? words[1] : words[0];

            if (initials.Length > _singleton._initialLength)
                return initials.Substring(0, _singleton._initialLength).ToUpper();
            else
                return initials.ToUpper();
        }

        [System.Serializable]
        public struct NumberNameStruct
        {
            public byte raceNumber;
            public string name;
        }

        [System.Serializable]
        public struct NumberSpriteStruct
        {
            public byte raceNumber;
            public Sprite sprite;
        }

        [System.Serializable]
        public struct TeamSpriteStruct
        {
            public Team team;
            public Sprite sprite;
        }
    }
}