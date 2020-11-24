using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace F1_Data_Management
{
    public static class TeamColor
    {
        static Dictionary<Team, Color> teamColor = new Dictionary<Team, Color>()
    {
        { Team.Mercedes_AMG_Petronas, new Color(8, 206, 178) },
        { Team.Ferrari, new Color(213, 1, 6) },
        { Team.Red_Bull_Racing, new Color(36, 64, 246) },
        { Team.Renault, new Color(241, 239, 16) },
        { Team.McLaren, new Color(252, 130, 6) },
        { Team.BWT_Racing_Point, new Color(233, 146, 187) },
        { Team.Haas, new Color(121, 120, 119) },
        { Team.Williams, new Color(1, 124, 245) },
        { Team.Alfa_Romeo_Racing, new Color(150, 0, 12) },
        { Team.AlphaTauri, new Color(249, 248, 245) }
    };

        static Color DEFAULT_COLOR = new Color(40, 40, 40);

        /// <summary>
        /// Returns color matched with team if available. Otherwise return default color.
        /// </summary>
        public static Color GetColorByTeam(Team team)
        {
            if (teamColor.ContainsKey(team))
                return teamColor[team];
            else
                return DEFAULT_COLOR;
        }
    }
}