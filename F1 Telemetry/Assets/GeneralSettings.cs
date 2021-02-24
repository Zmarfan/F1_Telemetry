using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using F1_Data_Management;
using FileExplorer;
using System.Linq;

namespace F1_Options
{
    public class GeneralSettings : MonoBehaviour
    {
        [SerializeField] List<StringTrackStruct> _trackNames;
        [SerializeField] GameObject _trackNamePrefab;
        [SerializeField] Transform _contentParent;

        Dictionary<Track, StringTrackStruct> _trackNameDictionary = new Dictionary<Track, StringTrackStruct>();

        List<TrackText> _trackTextList = new List<TrackText>();

        public void Init()
        {
            SetupTrackNameDictionary();
            LoadFromMemory();
            SpawnTrackTexts();
        }

        /// <summary>
        /// Initilizes dictionary to be able to bigO(1) access of track text given a track
        /// </summary>
        void SetupTrackNameDictionary()
        {
            for (int i = 0; i < _trackNames.Count; i++)
                _trackNameDictionary.Add(_trackNames[i].track, _trackNames[i]);
        }

        /// <summary>
        /// Load in all player selected track strings from storage and set it for each track
        /// </summary>
        void LoadFromMemory()
        {
            Array array = Enum.GetValues(typeof(Track));
            for (int i = 0; i < array.Length; i++)
            {
                Track track = (Track)array.GetValue(i);
                object data = SaveSystem.Load(track.ToString());

                StringTrackStruct trackData = _trackNameDictionary[track];
                string text = trackData.defaultText;
                if (data != null)
                    text = (string)data;

                trackData.text = text;
                _trackNameDictionary[track] = trackData;
            }
        }

        /// <summary>
        /// Spawn option prefab for all tracks to be able to change name. Stores these scripts in list to be able to use entered data later
        /// </summary>
        void SpawnTrackTexts()
        {
            List<StringTrackStruct> allTrackTexts = _trackNameDictionary.Values.ToList();
            foreach (StringTrackStruct trackText in allTrackTexts)
                _trackTextList.Add(SpawnTrackTextPrefab(trackText));
        }

        /// <summary>
        /// Spawn and init a track specific setting prefab
        /// </summary>
        TrackText SpawnTrackTextPrefab(StringTrackStruct trackText)
        {
            GameObject obj = Instantiate(_trackNamePrefab, Vector3.zero, Quaternion.identity, _contentParent) as GameObject;
            TrackText script = obj.GetComponent<TrackText>();
            script.Init(trackText);
            return script;
        }
    }

    /// <summary>
    /// Used to initilize dictionary of string in inspector with tracks as keys
    /// </summary>
    [System.Serializable]
    public struct StringTrackStruct
    {
        public Track track;
        public string defaultText;
        public string text;
        public Sprite flag;
    }
}