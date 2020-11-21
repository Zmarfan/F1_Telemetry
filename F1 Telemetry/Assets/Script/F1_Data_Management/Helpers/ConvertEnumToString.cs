using System;

namespace F1_Data_Management
{
    /// <summary>
    /// Used to convert enums to strings -> remove dashes
    /// </summary>
    public static class ConvertEnumToString
    {
        /// <summary>
        /// Removes all '_' in enum names
        /// </summary>
        public static string Convert<T>(T enumInstance)
        {
            try { Enum.IsDefined(typeof(T), enumInstance); }
            catch { throw new System.Exception("Only enums can be converted to strings"); }
            return enumInstance.ToString().Replace('_', ' ');
        }
    }
}