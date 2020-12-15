using System.Collections.Generic;
using UnityEngine;
using F1_Data_Management;

namespace F1_Unity
{
    /// <summary>
    /// Holds all raceNumber to raceDriver correlation. Add multiplayer names here to available numbers.
    /// </summary>
    public class ParticipantManager : MonoBehaviour
    {
        [Header("Defaults")]

        [SerializeField] string _defaultDriverName = "Driver #";
        [SerializeField] string _defaultDriverInital = "#";
        [SerializeField] Sprite _defaultPortrait;
        [SerializeField] Sprite _defaultTeamSprite;
        [SerializeField] Sprite _defaultCarSprite;

        [Header("Lists")]

        [SerializeField] NumberNameStruct[] _defaultNames;
        [SerializeField] TeamSpriteStruct[] _teamSpriteList;
        [SerializeField] TeamSpriteStruct[] _carSpriteList;
        [SerializeField] VisualCompoundSpriteStruct[] _visualTyreCompounds;

        static ParticipantManager _singleton;
        static Dictionary<byte, NumberNameStruct> _namesByRaceNumber = new Dictionary<byte, NumberNameStruct>();
        static Dictionary<byte, Sprite> _portraitByRaceNumber = new Dictionary<byte, Sprite>();
        static Dictionary<Team, Sprite> _teamSpriteByTeam = new Dictionary<Team, Sprite>();
        static Dictionary<Team, Sprite> _carSpriteByTeam = new Dictionary<Team, Sprite>();
        static Dictionary<VisualTyreCompound, Sprite> _visualTyreCompoundSpriteByEnum = new Dictionary<VisualTyreCompound, Sprite>();

        private void Awake()
        {
            if (_singleton == null)
                Init();
            else
                Destroy(this.gameObject);
        }

        /// <summary>
        /// Sets all lists that user have control over, is called before Awake is called
        /// </summary>
        public void Init(List<NumberNameStruct> numberNameList, List<NumberSpriteStruct> portraitSprites)
        {
            //Names
            for (int i = 0; i < numberNameList.Count; i++)
                _namesByRaceNumber.Add(numberNameList[i].raceNumber, numberNameList[i]);
            //Add default names if number isn't already taken
            for (int i = 0; i < _defaultNames.Length; i++)
            {
                if (!_namesByRaceNumber.ContainsKey(_defaultNames[i].raceNumber))
                    _namesByRaceNumber.Add(_defaultNames[i].raceNumber, _defaultNames[i]);
            }

            //Portraits
            for (int i = 0; i < portraitSprites.Count; i++)
                _portraitByRaceNumber.Add(portraitSprites[i].raceNumber, portraitSprites[i].sprite);
        }

        /// <summary>
        /// Make a dictionary of list data for easy access
        /// </summary>
        void Init()
        {
            _singleton = this;
            
            //Team logos
            for (int i = 0; i < _teamSpriteList.Length; i++)
                _teamSpriteByTeam.Add(_teamSpriteList[i].team, _teamSpriteList[i].sprite);
            //Team car
            for (int i = 0; i < _carSpriteList.Length; i++)
                _carSpriteByTeam.Add(_carSpriteList[i].team, _carSpriteList[i].sprite);
            //Visual Tyre sprite
            for (int i = 0; i < _visualTyreCompounds.Length; i++)
                _visualTyreCompoundSpriteByEnum.Add(_visualTyreCompounds[i].compound, _visualTyreCompounds[i].sprite);
        }

        /// <summary>
        /// Converts raceNumber to race driver name, returns "Driver #raceNumber" if not in system yet
        /// </summary>
        public static string GetNameFromNumber(byte raceNumber)
        {
            if (_namesByRaceNumber.ContainsKey(raceNumber))
                return _namesByRaceNumber[raceNumber].name;
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
            if (_namesByRaceNumber.ContainsKey(raceNumber))
                return _namesByRaceNumber[raceNumber].initals;
            else
                return _singleton._defaultDriverInital + raceNumber.ToString();
        }

        /// <summary>
        /// Returns sprite of visual tyre compound.
        /// </summary>
        public static Sprite GetVisualTyreCompoundSprite(VisualTyreCompound visualTyreCompound)
        {
            return _visualTyreCompoundSpriteByEnum[visualTyreCompound];
        }

        [System.Serializable]
        public struct NumberNameStruct
        {
            public byte raceNumber;
            public string name;
            public string initals;
        }

        [System.Serializable]
        public struct NumberSpriteStruct
        {
            public byte raceNumber;
            public Sprite sprite;

            public NumberSpriteStruct(Sprite sprite, byte raceNumber)
            {
                this.raceNumber = raceNumber;
                this.sprite = sprite;
            }
        }

        [System.Serializable]
        public struct TeamSpriteStruct
        {
            public Team team;
            public Sprite sprite;
        }

        [System.Serializable]
        public struct VisualCompoundSpriteStruct
        {
            public VisualTyreCompound compound;
            public Sprite sprite;
        }
    }
}