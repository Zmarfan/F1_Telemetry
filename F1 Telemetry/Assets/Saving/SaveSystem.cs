using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace FileExplorer
{
    public static class SaveSystem
    {
        static readonly string PARTICIPANT_DATA_NAME = "/ParticipantData.pog";

        /// <summary>
        /// Saves the selected filepath for participant data .txt for easy access in future
        /// </summary>
        /// <param name="filePath">Filepath for text file</param>
        public static void SaveParticipantTextFilePath(string saveData)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            string filePath = Application.persistentDataPath + PARTICIPANT_DATA_NAME;
            FileStream stream = new FileStream(filePath, FileMode.Create);

            formatter.Serialize(stream, saveData);
            stream.Close();
        }

        /// <summary>
        /// Loads in filepath for participant data text file.
        /// </summary>
        /// <returns></returns>
        public static string LoadParticipantTextFilePath()
        {
            string filePath = Application.persistentDataPath + PARTICIPANT_DATA_NAME;
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
                Debug.LogError("Save file not found in: " + filePath);
                return null;
            }
        }
    }
}