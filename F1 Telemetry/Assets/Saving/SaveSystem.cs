using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace FileExplorer
{
    /// <summary>
    /// Different things saved for application
    /// </summary>
    public enum SaveTypes
    {
        ParticipantData,
        Portrait,
        ChampionshipData
    }

    public static class SaveSystem
    {
        static readonly string PARTICIPANT_DATA_NAME = "/ParticipantData.pog";
        static readonly string PORTRAIT_DATA_NAME = "/PortraitData.pog";
        static readonly string CHAMPIONSHIP_DATA_NAME = "/ChampionshipData.pog";

        /// <summary>
        /// Returns filename for a specific save type
        /// </summary>
        static string GetTypeName(SaveTypes type)
        {
            switch (type)
            {
                case SaveTypes.ParticipantData:  return PARTICIPANT_DATA_NAME;
                case SaveTypes.Portrait:         return PORTRAIT_DATA_NAME;
                case SaveTypes.ChampionshipData: return CHAMPIONSHIP_DATA_NAME;
                default: throw new System.Exception("This save type is not implemented yet!");
            }
        }

        /// <summary>
        /// Saves the selected filepath for participant data .txt for easy access in future
        /// </summary>
        /// <param name="filePath">Filepath for text file</param>
        public static void SaveFilePath(string saveData, SaveTypes type)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            string filePath = Application.persistentDataPath + GetTypeName(type);
            FileStream stream = new FileStream(filePath, FileMode.Create);

            formatter.Serialize(stream, saveData);
            stream.Close();
        }

        /// <summary>
        /// Loads in filepath for participant data text file.
        /// </summary>
        /// <returns></returns>
        public static string LoadFilePath(SaveTypes type)
        {
            string filePath = Application.persistentDataPath + GetTypeName(type);
            if (File.Exists(filePath))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(filePath, FileMode.Open);
                string returnValue = (string)formatter.Deserialize(stream);
                stream.Close();
                return returnValue;
            }
            else
            {
                Debug.LogWarning(type + " save file not found in: " + filePath);
                return null;
            }
        }
    }
}