using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles byte array data and allow traversal within it
/// </summary>
public class ByteManager
{
    byte[] _data;
    int _index;

    /// <summary>
    /// Input data data and index to start reading that data from
    /// </summary>
    public ByteManager(byte[] data, int startIndex = 0)
    {
        if (startIndex < 0)
            throw new System.IndexOutOfRangeException("startIndex must be a non negative integer!");
        if (startIndex >= data.Length)
            throw new System.IndexOutOfRangeException("startIndex exceeds data length");

        this._data = data;
        this._index = startIndex;
    }

    /// <summary>
    /// Traverses data forward with amountOfBytes bytes and return those bytes
    /// </summary>
    public byte[] GetBytes(int amountOfBytes)
    {
        byte[] returnBytes = new byte[amountOfBytes];

        for (int i = 0; i < amountOfBytes; i++)
            returnBytes[i] = _data[_index + i];

        _index += amountOfBytes;
        return returnBytes;
    }

    /// <summary>
    /// Returns the next byte in data and moves along data one step
    /// </summary>
    /// <returns></returns>
    public byte GetByte()
    {
        byte returnByte = _data[_index];
        _index++;
        return returnByte;
    }
}
