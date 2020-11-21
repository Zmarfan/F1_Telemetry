using System.Text;

namespace F1_Data_Management
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