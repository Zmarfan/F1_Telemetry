using UnityEngine;
using System.Collections.Generic;
using F1_Data_Management;

namespace F1_Unity
{
    public class TeamColor : MonoBehaviour
    {
        [SerializeField] Color _defaultColor = new Color(40, 40, 40);
        [SerializeField] TeamColorStruct[] _teamColors;

        static TeamColor _singleton;
        static Dictionary<Team, Color> teamColor = new Dictionary<Team, Color>();

        private void Awake()
        {
            if (_singleton == null)
                Init();
            else
                Destroy(this.gameObject);
        }

        /// <summary>
        /// Makes list into dictionary for easy access
        /// </summary>
        void Init()
        {
            _singleton = this;
            for (int i = 0; i < _teamColors.Length; i++)
                teamColor.Add(_teamColors[i].team, _teamColors[i].color);
        }

        /// <summary>
        /// Returns color matched with team if available. Otherwise return default color.
        /// </summary>
        public static Color GetColorByTeam(Team team)
        {
            if (teamColor.ContainsKey(team))
                return teamColor[team];
            else
                return _singleton._defaultColor;
        }

        [System.Serializable]
        public struct TeamColorStruct
        {
            public Team team;
            public Color color;
        }
    }
}