﻿using UnityEngine;
using F1_Data_Management;
using System.Collections.Generic;

namespace F1_Unity
{
    public class FlagManager : MonoBehaviour
    {
        [SerializeField] FlagNationStruct[] _flagsByNation;
        [SerializeField] FlagTrackStruct[] _flagsByTrack;
        [SerializeField] StringTrackStruct[] _stringByTrack;

        static FlagManager _singleton;
        static Dictionary<Nationality, Sprite> _flagSpritesByNationality = new Dictionary<Nationality, Sprite>();
        static Dictionary<Track, Sprite> _flagSpritesByTrack = new Dictionary<Track, Sprite>();
        static Dictionary<Track, string> _grandPrixStringByTrack = new Dictionary<Track, string>();

        private void Awake()
        {
            if (_singleton == null)
                Init();
            else
                Destroy(this.gameObject);
        }

        void Init()
        {
            _singleton = this;

            for (int i = 0; i < _flagsByNation.Length; i++)
                _flagSpritesByNationality.Add(_flagsByNation[i].nationality, _flagsByNation[i].flagSprite);
            for (int i = 0; i < _flagsByTrack.Length; i++)
                _flagSpritesByTrack.Add(_flagsByTrack[i].track, _flagsByTrack[i].flagSprite);
            for (int i = 0; i < _stringByTrack.Length; i++)
                _grandPrixStringByTrack.Add(_stringByTrack[i].track, _stringByTrack[i].text);
        }

        /// <summary>
        /// Returns flag sprite given nationality.
        /// </summary>
        /// <param name="nationality">What flag?</param>
        /// <returns>Sprite of that flag</returns>
        public static Sprite GetFlag(Nationality nationality)
        {
            return _flagSpritesByNationality[nationality];
        }

        /// <summary>
        /// Returns flag for specific track
        /// </summary>
        public static Sprite GetFlagByTrack(Track track)
        {
            return _flagSpritesByTrack[track];
        }

        /// <summary>
        /// Returns Grand Prix String for specific track
        /// </summary>
        public static string GetGrandPrixString(Track track)
        {
            return _grandPrixStringByTrack[track];
        }

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
        /// Used to initilize dictionary of string in inspector with tracks as keys
        /// </summary>
        [System.Serializable]
        public struct StringTrackStruct
        {
            public Track track;
            public string text;
        }
    }
}