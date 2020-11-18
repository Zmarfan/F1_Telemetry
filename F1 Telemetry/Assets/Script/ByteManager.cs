using System;
using System.Text;

/// <summary>
/// Handles byte array data and allow traversal within it
/// </summary>
public class ByteManager
{
    public readonly int STATEMENT_TRUE = 1;

    byte[] _data;
    int _index;
    string _debugData;

    /// <summary>
    /// Input data data and index to start reading that data from
    /// </summary>
    public ByteManager(byte[] data, int startIndex = 0, string debugData = "")
    {
        this._data = data;
        this._index = startIndex;
        this._debugData = debugData;

        CheckIndexValidity(startIndex);
    }

    /// <summary>
    /// Returns the index manager is currently at
    /// </summary>
    public int CurrentIndex { get { return _index; } }

    /// <summary>
    /// Sets a new index for an existing manager
    /// </summary>
    public void SetNewIndex(int newIndex)
    {
        CheckIndexValidity(newIndex);
        _index = newIndex;
    }

    #region MainDataTraversers

    /// <summary>
    /// Traverses data forward with amountOfBytes bytes and return those bytes
    /// </summary>
    public byte[] GetBytes(int amountOfBytes)
    {
        CheckIndexValidity(_index + amountOfBytes);

        byte[] returnBytes = new byte[amountOfBytes];

        for (int i = 0; i < amountOfBytes; i++)
            returnBytes[i] = _data[_index + i];

        _index += amountOfBytes;
        return returnBytes;
    }

    /// <summary>
    /// Returns the next byte in data and moves along data one step
    /// </summary>
    public byte GetByte()
    {
        CheckIndexValidity(_index + 1);

        byte returnByte = _data[_index];
        _index++;
        return returnByte;
    }

    /// <summary>
    /// Used when a specific byte known to the user is to be reached
    /// </summary>
    public byte GetByteAt(int index)
    {
        CheckIndexValidity(index);
        return _data[index];
    }

    #endregion

    #region GetDifferentTypes

    /// <summary>
    /// Returns the next byte as signed in data and moves along data one step
    /// </summary>
    public sbyte GetSignedByte()
    {
        return (sbyte)GetByte();
    }

    /// <summary>
    /// Returns the next 4 bytes as uint in data and moves along data four step
    /// </summary>
    public uint GetUnsignedInt()
    {
        return BitConverter.ToUInt32(GetBytes(sizeof(uint)), 0);
    }

    /// <summary>
    /// Returns the next 2 bytes as short in data and moves along data two step
    /// </summary>
    public short GetShort()
    {
        return BitConverter.ToInt16(GetBytes(sizeof(short)), 0);
    }

    /// <summary>
    /// Returns the next 2 bytes as unsigned short in data and moves along data two step
    /// </summary>
    public ushort GetUnsignedShort()
    {
        return BitConverter.ToUInt16(GetBytes(sizeof(ushort)), 0);
    }

    /// <summary>
    /// Returns the next 4 bytes as float in data and moves along data four step
    /// </summary>
    public float GetFloat()
    {
        return BitConverter.ToSingle(GetBytes(sizeof(float)), 0);
    }

    /// <summary>
    /// Returns the next 8 bytes as ulong in data and moves along data eight step
    /// </summary>
    public ulong GetUnsignedLong()
    {
        return BitConverter.ToUInt64(GetBytes(sizeof(ulong)), 0);
    }

    /// <summary>
    /// Returns the next "stringLength" bytes as string in data and moves along data stringLength step
    /// </summary>
    public string GetString(int stringLength)
    {
        return Encoding.UTF8.GetString(GetBytes(stringLength));
    }

    /// <summary>
    /// Returns the next byte as bool in data and moves along data 1 step.
    /// 1 is considered true and everything else is considered false.
    /// </summary>
    public bool GetBool()
    {
        return GetByte() == STATEMENT_TRUE;
    }

    #endregion

    /// <summary>
    /// Checks weather parameter "index" is within data range, throw error if not
    /// </summary>
    void CheckIndexValidity(int index)
    {
        if (index < 0)
            throw new System.IndexOutOfRangeException(_debugData + ", startIndex must be a non negative integer!");
        if (index >= _data.Length)
            throw new System.IndexOutOfRangeException(_debugData + ", startIndex exceeds data length");
    }
}
