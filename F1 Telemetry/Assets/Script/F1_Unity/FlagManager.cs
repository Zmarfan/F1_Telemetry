using UnityEngine;
using F1_Data_Management;
using System.Collections.Generic;
using F1_Options;

namespace F1_Unity
{
    public class FlagManager : MonoBehaviour
    {
        [SerializeField] FlagNationStruct[] _flagsByNation;
        [SerializeField] FlagTrackStruct[] _flagsByTrack;
        [SerializeField] StringTrackStruct[] _stringByTrack;
        [SerializeField] CircuitInfoData[] _circuitInfoData;
        [SerializeField] SpriteWeatherStruct[] _weatherSpriteList;

        Dictionary<Nationality, Sprite> _flagSpritesByNationality = new Dictionary<Nationality, Sprite>();
        Dictionary<Track, Sprite> _flagSpritesByTrack = new Dictionary<Track, Sprite>();
        Dictionary<Track, string> _grandPrixStringByTrack = new Dictionary<Track, string>();
        Dictionary<Track, CircuitInfoData> _circuitInfoByTrack = new Dictionary<Track, CircuitInfoData>();
        Dictionary<Weather, Sprite> _spriteByWeather = new Dictionary<Weather, Sprite>();

        private void Awake()
        {
            Init();
        }

        void Init()
        {
            for (int i = 0; i < _flagsByNation.Length; i++)
                _flagSpritesByNationality.Add(_flagsByNation[i].nationality, _flagsByNation[i].flagSprite);
            for (int i = 0; i < _flagsByTrack.Length; i++)
                _flagSpritesByTrack.Add(_flagsByTrack[i].track, _flagsByTrack[i].flagSprite);
            for (int i = 0; i < _stringByTrack.Length; i++)
                _grandPrixStringByTrack.Add(_stringByTrack[i].track, _stringByTrack[i].defaultText);
            for (int i = 0; i < _circuitInfoData.Length; i++)
                _circuitInfoByTrack.Add(_circuitInfoData[i].track, _circuitInfoData[i]);
            for (int i = 0; i < _weatherSpriteList.Length; i++)
                _spriteByWeather.Add(_weatherSpriteList[i].weather, _weatherSpriteList[i].sprite);
        }

        /// <summary>
        /// Returns flag sprite given nationality.
        /// </summary>
        /// <param name="nationality">What flag?</param>
        /// <returns>Sprite of that flag</returns>
        public Sprite GetFlag(Nationality nationality)
        {
            return _flagSpritesByNationality[nationality];
        }

        /// <summary>
        /// Returns flag for specific track
        /// </summary>
        public Sprite GetFlagByTrack(Track track)
        {
            return _flagSpritesByTrack[track];
        }

        /// <summary>
        /// Returns Grand Prix String for specific track
        /// </summary>
        public string GetGrandPrixString(Track track)
        {
            return _grandPrixStringByTrack[track];
        }

        /// <summary>
        /// Gets weather sprite for a specific weather type
        /// </summary>
        public Sprite GetWeatherSprite(Weather weather)
        {
            return _spriteByWeather[weather];
        }

        /// <summary>
        /// Gets circuit info for a specific track. Track type, full throttle, downforce etc
        /// </summary>
        public CircuitInfoData GetCircuitInfoData(Track track)
        {
            return _circuitInfoByTrack[track];
        }

        #region Structs & Enums

        /// <summary>
        /// Used to initilize dictionary of flag in inspector with nationality as keys
        /// </summary>
        [System.Serializable]
        public struct FlagNationStruct
        {
            public Nationality nationality;
            public Sprite flagSprite;
        }

        /// <summary>
        /// Used to initilize dictionary of flag in inspector with tracks as keys
        /// </summary>
        [System.Serializable]
        public struct FlagTrackStruct
        {
            public Track track;
            public Sprite flagSprite;
        }

        /// <summary>
        /// Used to initilize dictionary of weather sprites in inspector with weather types as keys
        /// </summary>
        [System.Serializable]
        public struct SpriteWeatherStruct
        {
            public Weather weather;
            public Sprite sprite;
        }

        /// <summary>
        /// Holds general info about a specific circuit
        /// </summary>
        [System.Serializable]
        public struct CircuitInfoData
        {
            /// <summary>
            /// What track are these info for
            /// </summary>
            public Track track;
            /// <summary>
            /// What sort of speed is this track in
            /// </summary>
            public TrackType trackType;
            /// <summary>
            /// How much of the lap is spent on full throttle?
            /// </summary>
            public float fullThrottle;
            /// <summary>
            /// Top possible speed in km/h
            /// </summary>
            public ushort topSpeed;
            /// <summary>
            /// How much downforce is required to race this track
            /// </summary>
            public Downforce downforce;
            /// <summary>
            /// How much tyre wear is it on this track
            /// </summary>
            public TyreWear tyreWear;
        }

        /// <summary>
        /// What type of speed is a track in
        /// </summary>
        public enum TrackType
        {
            Slow,
            Medium,
            Fast
        }

        /// <summary>
        /// What sort of downforce is required for this track
        /// </summary>
        public enum Downforce
        {
            Low,
            Medium,
            High
        }

        /// <summary>
        /// How much tyre wear is it on this track
        /// </summary>
        public enum TyreWear
        {
            Low,
            Medium,
            High
        }

        #endregion
    }
}