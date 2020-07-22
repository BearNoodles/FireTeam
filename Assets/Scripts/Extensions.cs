using UnityEngine;
using System.Collections;
using System;

public static class Extensions {

    public static string ColorAsHex(this Color color)
    {
        return String.Format("#{0:x2}{1:x2}{2:x2}", ToByte(color.r), ToByte(color.g), ToByte(color.b));
    }

    private static byte ToByte(float value)
    {
        value = Mathf.Clamp01(value);
        return (byte)(value * 255);
    }
}
