using UnityEngine;
using System.Collections.Generic;
using F1_Data_Management;
using System;
using System.Text;

namespace F1_Unity
{
    public class F1Utility : MonoBehaviour
    {
        [SerializeField] Color _defaultColor = new Color(40, 40, 40);
        [SerializeField] TeamColorStruct[] _teamColors;

        static F1Utility _singleton;
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

        /// <summary>
        /// Converts seconds to +/-minute:seconds:millieseconds. If time is positive it will treat it as + 
        /// </summary>
        public static string GetDeltaStringSigned(float time)
        {
            string deltaString = GetDeltaString(Mathf.Abs(time));
            if (time > 0)
                return "+" + deltaString;
            return "-" + deltaString;
        }

        /// <summary>
        /// Converts seconds to minute:seconds:millieseconds
        /// </summary>
        public static string GetDeltaString(float time)
        {
            TimeSpan span = TimeSpan.FromSeconds(time);
            StringBuilder builder = new StringBuilder();
            if (span.Minutes > 0)
            {
                builder.Append(span.Minutes);
                builder.Append(':');
                builder.Append(span.Seconds.ToString("0#")); //Start with zero if one digit long
            }
            else
                builder.Append(span.Seconds);

            builder.Append('.');
            builder.Append(span.Milliseconds.ToString("000")); //Appends with 3 decimals

            return builder.ToString();
        }

        [System.Serializable]
        public struct TeamColorStruct
        {
            public Team team;
            public Color color;
        }
    }
}