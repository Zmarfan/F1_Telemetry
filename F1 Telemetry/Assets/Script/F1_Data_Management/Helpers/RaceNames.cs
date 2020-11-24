using System.Collections.Generic;

namespace F1_Data_Management
{
    /// <summary>
    /// Holds all raceNumber to raceDriver correlation. Add multiplayer names here to available numbers.
    /// </summary>
    public static class RaceNames
    {
        static readonly string DEFAULT_DRIVER_NAME = "Driver #";
        static readonly char[] SPLITTERS = new char[] { ' ', '_', '-' };
        static readonly byte INITIAL_LENGTH = 3;

        static Dictionary<byte, string> namesByRaceNumber = new Dictionary<byte, string>()
    {
        //2

        /////
        { 2, "iwasborntolose" }, //ADDED
        /////

        { 3, "Daniel Ricciardo" },
        { 4, "Lando Norris" },
        { 5, "Sebastian Vettel" },
        { 6, "Nicholas Latifi" },
        { 7, "Kimi Räikkönen" },
        { 8, "Romain Grosjean" },
        //9
        { 10, "Pierre Gasly" },
        { 11, "Sergio Pérez" },
        //12 - 15

        /////
        { 12, "Elias1337" }, //ADDED
        /////

        /////
        { 14, "PRL Faikoways" }, //ADDED
        /////

        /////
        { 15, "KingWunder" }, //ADDED
        /////

        { 16, "Charles Leclerc" },
        { 17, "Jules Bianchi" },
        { 18, "Lance Stroll" },
        //19
        { 20, "Kevin Magnussen" },
        //21 - 22

        /////
        { 21, "PRL Marijn" }, //ADDED
        /////

        /////
        { 22, "M4RCC" }, //ADDED
        /////

        { 23, "Alexander Albon" },
        //24 - 25

        /////
        { 24, "MR-" }, //ADDED
        /////

        { 26, "Daniil Kvyat" },
        //27 - 30

        /////
        { 27, "Stolly" }, //ADDED
        /////

        /////
        { 28, "BlinkdeVinkje" }, //ADDED
        /////

        /////
        { 30, "PRL Tekashi" }, //ADDED
        /////

        { 31, "Esteban Ocon" },
        //32
        { 33, "Max Verstappen" },
        //34 - 43

        /////
        { 34, "PRL daBurLs" }, //ADDED
        /////

        /////
        { 42, "PRL Pierre" }, //ADDED
        /////

        /////
        { 43, "TrueDoe" }, //ADDED
        /////

        { 44, "Lewis Hamilton" },
        //45 - 54
        { 55, "Carlos Sainz Jr." },
        //56 - 62

        /////
        { 57, "PRL Winston" }, //ADDED
        /////

        { 63, "George Russell" },
        //64 - 76

        /////
        { 64, "Psylexx" }, //ADDED
        /////

        /////
        { 67, "Svenna" }, //ADDED
        /////

        /////
        { 69, "PRL Fysq" }, //ADDED
        /////

        { 77, "Valtteri Bottas" },
        //78 - 98

        /////
        { 82, "Lord_Zmarfan" }, //ADDED
        /////

        /////
        { 93, "Robbie Van" }, //ADDED
        /////

        /////
        { 98, "Akhatal" }, //ADDED
        /////

        { 99, "Antonio Giovinazzi" },
    };

        /// <summary>
        /// Converts raceNumber to race driver name, returns "Driver #raceNumber" if not in system yet
        /// </summary>
        public static string GetNameFromNumber(byte raceNumber)
        {
            if (namesByRaceNumber.ContainsKey(raceNumber))
                return namesByRaceNumber[raceNumber];
            else
                return DEFAULT_DRIVER_NAME + raceNumber.ToString();
        }

        /// <summary>
        /// <para> Returns 3 first letters in second name. Dashes/Underscores are treated as spaces. </para>
        /// If only one name -> first 3 letters in that.
        /// </summary>
        public static string GetDriverInitials(byte raceNumber)
        {
            string fullName = GetNameFromNumber(raceNumber);
            string[] words = fullName.Split(SPLITTERS);

            //Get second word (surname) if available, otherwise take first word (firstName / Username for multiplayer)
            string initials = words.Length > 1 ? words[1] : words[0];

            if (initials.Length > INITIAL_LENGTH)
                return initials.Substring(0, INITIAL_LENGTH).ToUpper();
            else
                return initials.ToUpper();
        }
    }
}