using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;

namespace FileExplorer
{
    public static class SaveSystem
    {
        public static readonly string SAVE_FOLDER_PATH = Application.persistentDataPath + "/SaveData";
        public static readonly string DATA_ENDING = ".save";

        /// <summary>
        /// Saves specified data indexed by specified name
        /// </summary>
        /// <param name="saveName">Name to save and load this data</param>
        /// <param name="data">Data to save</param>
        /// <returns>Returns true when done saving</returns>
        public static bool Save(string saveName, object data)
        {
            BinaryFormatter formatter = CreateBinaryFormatter();

            //Create folder for saving if one doesn't exist
            if (!Directory.Exists(SAVE_FOLDER_PATH))
                Directory.CreateDirectory(SAVE_FOLDER_PATH);

            string path = SAVE_FOLDER_PATH + "/" + saveName + DATA_ENDING;
            FileStream file = File.Create(path);
            formatter.Serialize(file, data);
            file.Close();
            return true;
        }

        /// <summary>
        /// Returns loaded data indexed by saveName. Null if data don't exist or is damaged
        /// </summary>
        /// <param name="saveName">Name to save and load this data</param>
        /// <returns>Cast return data to expected type</returns>
        public static object Load(string saveName)
        {
            string path = SAVE_FOLDER_PATH + "/" + saveName + DATA_ENDING;
            if (!File.Exists(path))
                return null;

            BinaryFormatter formatter = CreateBinaryFormatter();
            FileStream file = File.Open(path, FileMode.Open);

            try
            {
                object data = formatter.Deserialize(file);
                file.Close();
                return data;
            }
            catch
            {
                file.Close();
                return null;
            }
        }

        /// <summary>
        /// Returns a binary formatter that can handle custom non serilized data types
        /// </summary>
        public static BinaryFormatter CreateBinaryFormatter()
        {
            BinaryFormatter formatter = new BinaryFormatter();

            SurrogateSelector selector = new SurrogateSelector();

            ColorSerilizationSurrogate colorSurrogate = new ColorSerilizationSurrogate();

            selector.AddSurrogate(typeof(Color), new StreamingContext(StreamingContextStates.All), colorSurrogate);

            formatter.SurrogateSelector = selector;
            return formatter;
        }

        public class ColorSerilizationSurrogate : ISerializationSurrogate
        {
            public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
            {
                Color color = (Color)obj;
                info.AddValue("r", color.r);
                info.AddValue("g", color.g);
                info.AddValue("b", color.b);
                info.AddValue("a", color.a);
            }

            public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
            {
                Color color = (Color)obj;
                color.r = (float)info.GetValue("r", typeof(float));
                color.g = (float)info.GetValue("g", typeof(float));
                color.b = (float)info.GetValue("b", typeof(float));
                color.a = (float)info.GetValue("a", typeof(float));
                obj = color;
                return obj;
            }
        }
    }
}