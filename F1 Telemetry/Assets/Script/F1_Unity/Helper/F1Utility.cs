﻿using UnityEngine;
using System.Collections.Generic;
using F1_Data_Management;
using System;
using System.Text;
using F1_Options;

namespace F1_Unity
{
    public class F1Utility : MonoBehaviour
    {
        [SerializeField] Color _defaultColor = new Color(40, 40, 40);

        Dictionary<Team, TeamColorData> _teamColor = new Dictionary<Team, TeamColorData>();

        /// <summary>
        /// Sets the team color corrolation the application will run after
        /// </summary>
        /// <param name=""></param>
        public void SetTeamColors(Dictionary<Team, TeamColorData> data)
        {
            _teamColor = data;
        }

        /// <summary>
        /// Returns color matched with team if available. Otherwise return default color.
        /// </summary>
        public Color GetColorByTeam(Team team)
        {
            if (_teamColor.ContainsKey(team))
                return _teamColor[team].currentColor;
            else
                return _defaultColor;
        }

        /// <summary>
        /// Converts seconds to +/-minute:seconds:millieseconds. If time is positive it will treat it as +
        /// <param name="time">Time in seconds</param>
        /// <param name="amountOfDecimals">How many decimals milliseconds will be represented as. Max 3</param>
        /// </summary>
        public static string GetDeltaStringSigned(float time, byte amountOfDecimals = 3)
        {
            string deltaString = GetDeltaString(Mathf.Abs(time), amountOfDecimals);
            if (time > 0)
                return "+" + deltaString;
            return "-" + deltaString;
        }

        /// <summary>
        /// Converts seconds to minute:seconds:millieseconds
        /// </summary>
        /// <param name="time">Time in seconds</param>
        /// <param name="amountOfDecimals">How many decimals milliseconds will be represented as. Max 3</param>
        /// <returns></returns>
        public static string GetDeltaString(float time, byte amountOfDecimals = 3)
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
            string milli = span.Milliseconds.ToString("00#");
            for (int i = 0; i < amountOfDecimals; i++)
                builder.Append(milli[i].ToString());

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