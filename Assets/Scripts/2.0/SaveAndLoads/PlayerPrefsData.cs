using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class PlayerPrefsData
{
    public List<IntPref> intPrefs = new List<IntPref>();
    public List<FloatPref> floatPrefs = new List<FloatPref>();
    public List<StringPref> stringPrefs = new List<StringPref>();
    public DateTime saveTime;
    public int dataVersion = 1;
}

[System.Serializable]
public class IntPref
{
    public string key;
    public int value;

    public IntPref(string key, int value)
    {
        this.key = key;
        this.value = value;
    }
}

[System.Serializable]
public class FloatPref
{
    public string key;
    public float value;

    public FloatPref(string key, float value)
    {
        this.key = key;
        this.value = value;
    }
}

[System.Serializable]
public class StringPref
{
    public string key;
    public string value;

    public StringPref(string key, string value)
    {
        this.key = key;
        this.value = value;
    }
}