using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
        { 16, "Charles Leclerc" },
        { 17, "Jules Bianchi" },
        { 18, "Lance Stroll" },
        //19
        { 20, "Kevin Magnussen" },
        //21 - 22
        { 23, "Alexander Albon" },
        //24 - 25
        { 26, "Daniil Kvyat" },
        //27 - 30
        { 31, "Esteban Ocon" },
        //32
        { 33, "Max Verstappen" },
        //34 - 43
        { 44, "Lewis Hamilton" },
        //45 - 54
        { 55, "Carlos Sainz Jr." },
        //56 - 62
        { 63, "George Russell" },
        //64 - 76
        { 77, "Valtteri Bottas" },
        //78 - 98
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

        return initials.Substring(0, INITIAL_LENGTH).ToUpper();
    }
}
