using UnityEngine;

public struct HSLColor
{
    public float h;
    public float s;
    public float l;

    public HSLColor(float h, float s, float l)
    {
        this.h = h;
        this.s = s;
        this.l = l;
    }

    /// <summary>
    /// Returns a RGB representation of this color
    /// </summary>
    public Color RGBColor()
    {
        //Modify hsl vector to easier create HSV vector
        float modH = h;
        float modL = l * 2;
        float modS = s * ((modL <= 1) ? modL : 2 - modL);

        //Generate a HSV vector from HSL vector -> takes less code and can utilize Unity function 
        float hsvH = modH;
        float hsvS = (2 * modS) / (modL + modS);
        float hsvV = (modL + modS) / 2;

        return Color.HSVToRGB(hsvH, hsvS, hsvV);
    }

    /// <summary>
    /// Creates a RGB color from HSL variables
    /// </summary>
    /// <param name="h">Hue</param>
    /// <param name="s">Saturation</param>
    /// <param name="l">lightness</param>
    public static Color RGBColor(float h, float s, float l)
    {
        HSLColor hslColor = new HSLColor(h, s, l);
        return hslColor.RGBColor();
    }

    /// <summary>
    /// Creates a HSL representation of a color from RGB color
    /// </summary>
    public static HSLColor HSLFromRGB(Color color)
    {
        float max = Mathf.Max(color.r, color.g, color.b);
        float min = Mathf.Min(color.r, color.g, color.b);
        float average = (max + min) / 2;
        HSLColor hslVector = new HSLColor(average, average, average);

        //Achromatic
        if (max == min)
        {
            hslVector.h = 0;
            hslVector.s = 0;
        }
        else
        {
            float d = max - min;
            hslVector.s = hslVector.l > 0.5f ? d / (2 - max - min) : d / (max + min);

            if (max == color.r)
                hslVector.h = (color.g - color.b) / d + (color.g < color.b ? 6 : 0);
            else if (max == color.g)
                hslVector.h = (color.b - color.r) / d + 2;
            else if (max == color.b)
                hslVector.h = (color.r - color.g) / d + 4;

            hslVector.h /= 6;
        }

        return hslVector;
    }
}
