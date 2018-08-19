using UnityEngine;
using System.Collections;
using System;

public class Util {
    // RGB value with components in range 0-255
    public static Color Color255(int r, int g, int b)
    {
        return new Color(r / 255f, g / 255f, b / 255f);
    }

    // X position in pixels of screen element given positioning factor from 0.0 - 1.0
    public static int ScreenPosX(float xFactor)
    {
        int margin = 120;
        return (int)(margin + ((MainScript.windowRes.x - margin * 2) * xFactor));
    }

    // MM:SS timestamp from number of seconds
    public static string MinuteTimeStampFromSeconds(int seconds)
    {
        DateTime dt_seconds = DateTime.Today.AddSeconds(seconds);
        return dt_seconds.ToString("mm:ss");
    }

    // JSON parsing
    public static float ParseJSONFloat(object obj)
    {
        if (obj is System.Double)
            return (float)((double)obj);
        return (long)obj;
    }

    public static bool ShiftDown()
    {
        return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
    }

    // Easing Functions
    public static float LerpLinearEase(float start, float end, float perc)
    {
        return (end - start) * perc + start;
    }

    public static float LerpQuadEaseIn(float start, float end, float perc)
    {
        return (end - start) * perc * perc + start;
    }

    public static float LerpQuadEaseOut(float start, float end, float perc)
    {
        return -(end - start) * perc * (perc - 2f) + start;
    }

    public static float LerpQuadEaseInOut(float start, float end, float perc)
    {
        if (perc < 0.5)
            return (end - start) * perc * perc * 2f + start;
        else
            return (end - start) * (-1 + (4-2*perc) * perc) + start;
    }

    public static float LerpSinEaseInOut(float start, float end, float perc)
    {
        return -(end - start) / 2f * (Mathf.Cos(perc * Mathf.PI) - 1f) + start;
    }

    public static float LerpCircEaseIn(float start, float end, float perc)
    {
        return (end - start) * (1 - Mathf.Sqrt(1 - perc*perc)) + start;
    }

    public static float LerpCircEaseOut(float start, float end, float perc)
    {
        return (end - start) * ((perc - 1) * Mathf.Sqrt(1 - perc*perc))  + start;
    }

    public static float LerpExpEaseIn(float start, float end, float perc)
    {
        //return (end - start) * Mathf.Pow(2, 10f * (perc - 1)) + start;
        return (end - start) * Mathf.Pow(perc, 5) + start;
    }

    public static float LerpExpEaseOut(float start, float end, float perc)
    {
        return (1 + (1-perc)*Mathf.Pow(perc, 4)) * (end-start) + start;
        //return (end - start) * (-Mathf.Pow(2, -10f * (perc - 1)) + 1f) + start;
    }
}
