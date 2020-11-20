using System.Collections;
using System.Collections.Generic;
using F1_Telemetry;
using System.Text;

public static class TeamColor
{
    static Dictionary<Team, Color> teamColor = new Dictionary<Team, Color>()
    {
        { Team.Mercedes, new Color(8, 206, 178) },
        { Team.Ferrari, new Color(213, 1, 6) },
        { Team.Red_Bull_Racing, new Color(36, 64, 246) },
        { Team.Renault, new Color(241, 239, 16) },
        { Team.McLaren, new Color(252, 130, 6) },
        { Team.Racing_Point, new Color(233, 146, 187) },
        { Team.Haas, new Color(121, 120, 119) },
        { Team.Williams, new Color(1, 124, 245) },
        { Team.Alfa_Romeo, new Color(150, 0, 12) },
        { Team.Alpha_Tauri, new Color(249, 248, 245) }
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

namespace F1_Telemetry
{
    /// <summary>
    /// Representation of RGBA Colors. Values are between 0.0f and 1.0f for each component.
    /// </summary>
    public struct Color
    {
        /// <summary>
        /// Alpha component of the color (0 is transparent, 1 is opaque).
        /// </summary>
        public float a { get; set; }
        /// <summary>
        /// Blue component of the color.
        /// </summary>
        public float b { get; set; }
        /// <summary>
        /// Green component of the color.
        /// </summary>
        public float g { get; set; }
        /// <summary>
        /// Red component of the color.
        /// </summary>
        public float r { get; set; }

        /// <summary>
        /// Construct new Color with r, g & b parameters. a = 1.0f.
        /// </summary>
        public Color(float r, float g, float b)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = 1.0f;
        }

        /// <summary>
        /// Construct new Color with r, g, b & a parameters.
        /// </summary>
        public Color(float r, float g, float b, float a)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }

        /// <summary>
        /// Construct new Color with r, g, b & a parameters as byte values between 0 and 255.
        /// </summary>
        public Color(byte r, byte g, byte b, byte a)
        {
            this.r = (float)r / byte.MaxValue;
            this.g = (float)g / byte.MaxValue;
            this.b = (float)b / byte.MaxValue;
            this.a = (float)a / byte.MaxValue;
        }

        /// <summary>
        /// Construct new Color with r, g, b parameters as byte values between 0 and 255. a is set to 1.0f
        /// </summary>
        public Color(byte r, byte g, byte b)
        {
            this.r = (float)r / byte.MaxValue;
            this.g = (float)g / byte.MaxValue;
            this.b = (float)b / byte.MaxValue;
            this.a = 1.0f;
        }

        /// <summary>
        /// returns "(r: r-value, g: g-value, b: b-value, a: a-value)"
        /// </summary>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("(r: ");
            builder.Append(r.ToString());
            builder.Append(", g: ");
            builder.Append(g.ToString());
            builder.Append(", b: ");
            builder.Append(b.ToString());
            builder.Append(", a: ");
            builder.Append(a.ToString());
            builder.Append(')');
            return builder.ToString();
        }
    }
}

