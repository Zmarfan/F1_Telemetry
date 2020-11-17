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
        this._data = data;
        this._index = startIndex;

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
    /// <returns></returns>
    public byte GetByte()
    {
        CheckIndexValidity(_index + 1);

        byte returnByte = _data[_index];
        _index++;
        return returnByte;
    }

    public byte GetByteAt(int index)
    {
        CheckIndexValidity(index);
        return _data[index];
    }

    void CheckIndexValidity(int index)
    {
        if (index < 0)
            throw new System.IndexOutOfRangeException("startIndex must be a non negative integer!");
        if (index >= _data.Length)
            throw new System.IndexOutOfRangeException("startIndex exceeds data length");
    }
}
