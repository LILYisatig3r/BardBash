using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets._Scripts;

public class KeyBuffer
{
    private S_Key[] buffer;
    private int iterator;

    public int Count
    {
        get { return iterator; }
    }

    public KeyBuffer(int capacity)
    {
        buffer = new S_Key[capacity];
        iterator = 0;
    }

    public bool Add(S_Key item)
    {
        if (iterator < buffer.Length)
        {
            buffer[iterator++] = item;
            return true;
        }
        return false;
    }

    public S_Key Remove()
    {
        if (iterator > 0)
        {
            S_Key ret = buffer[iterator];
            buffer[iterator--] = default(S_Key);
            return ret;
        }
        return default(S_Key);
    }

    public void Copy(ref KeyBuffer b)
    {
        if (buffer.Length != b.buffer.Length)
            throw new System.Exception("Buffers not of the same length!");

        for (int i = 0; i < buffer.Length; i++)
            b.buffer[i] = buffer[i];
        b.iterator = iterator;
    }

    public void AddFrom(KeyBuffer b)
    {
        int i = 0;
        while (b.buffer[i] != S_Key.None && Add(b.buffer[i++]))
            ;
    }

    public void Clear()
    {
        for (int i = 0; i < buffer.Length; i++)
            buffer[i] = default(S_Key);
        iterator = 0;
    }

    public S_Key And()
    {
        S_Key ret = default(S_Key);
        foreach (S_Key t in buffer)
            ret = ret & t;
        return ret;
    }

    public S_Key Or()
    {
        S_Key ret = default(S_Key);
        foreach (S_Key t in buffer)
            ret = ret | t;
        return ret;
    }
}
