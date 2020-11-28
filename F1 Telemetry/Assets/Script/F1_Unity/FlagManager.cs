using UnityEngine;
using F1_Data_Management;
using System.Collections.Generic;

namespace F1_Unity
{
    public class FlagManager : MonoBehaviour
    {
        [SerializeField] Flag[] _flags;

        static FlagManager _singleton;
        static Dictionary<Nationality, Sprite> _flagSprites = new Dictionary<Nationality, Sprite>();

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

            for (int i = 0; i < _flags.Length; i++)
                _flagSprites.Add(_flags[i].nationality, _flags[i].flagSprite);
        }

        /// <summary>
        /// Returns flag sprite given nationality.
        /// </summary>
        /// <param name="nationality">What flag?</param>
        /// <returns>Sprite of that flag</returns>
        public static Sprite GetFlag(Nationality nationality)
        {
            return _flagSprites[nationality];
        }

        /// <summary>
        /// Used to initilize dictionary of flag in inspector
        /// </summary>
        [System.Serializable]
        public struct Flag
        {
            public Nationality nationality;
            public Sprite flagSprite;
        }
    }
}