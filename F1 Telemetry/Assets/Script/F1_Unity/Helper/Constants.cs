namespace F1_Unity
{
    public static class Constants
    {
        public const float CONVERT_KMH_TO_MPH = 0.621371192f;

        public static float ConvertCelsiusToFahrenheit(float degreesCelsius)
        {
            return degreesCelsius * 1.8f + 32;
        }
    }
}