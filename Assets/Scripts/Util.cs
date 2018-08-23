using UnityEngine;
using System.Collections;
using System;

public class Util {
    // RGB value with components in range 0-255
    public static Color Color255(int r, int g, int b)
    {
        return new Color(r / 255f, g / 255f, b / 255f);
    }

    // Returns X position in pixels of a screen element given positioning factor from 0.0 - 1.0
    public static int ScreenPosX(float xFactor)
    {
        int margin = 120;
        return (int)(margin + ((VoezEditor.windowRes.x - margin * 2) * xFactor));
    }

    // Inverse of above function. Returns positioning factor from 0.0 - 1.0 based on X position in pixels of a screen element.
    public static float InvScreenPosX(float xPixels)
    {
        int margin = 120;
        float retval = (xPixels - margin) / (VoezEditor.windowRes.x - margin * 2);
        return Mathf.Clamp(retval, 0f, 1f);
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
        return (end - start) * (perc * perc) + start;
    }

    public static float LerpQuadEaseOut(float start, float end, float perc)
    {
        return -(end - start) * (perc * (perc - 2f)) + start;
    }

    public static float LerpQuadEaseInOut(float start, float end, float perc)
    {
        if (perc < 0.5)
            return (end - start) * perc * perc * 2f + start;
        else
            return (end - start) * (-1 + (4-2*perc) * perc) + start;
    }

    public static float LerpCircEaseIn(float start, float end, float perc)
    {
        return (end - start) * (1 - Mathf.Sqrt(1 - perc*perc)) + start;
    }

    public static float LerpCircEaseOut(float start, float end, float perc)
    {
        return -(end - start) * -Mathf.Sqrt(1 - (perc-1)*(perc-1))  + start;
    }

    public static float LerpExpEaseIn(float start, float end, float perc)
    {
        return (end - start) * Mathf.Pow(2, 10f * (perc - 1)) + start;
    }

    public static float LerpExpEaseOut(float start, float end, float perc)
    {
        return -(end - start) * (-1 - (-Mathf.Pow(2, -10f * perc))) + start;
    }

    public static float LerpBackEaseIn(float start, float end, float perc)
    {
        return (end - start) * (perc * perc * (2.70158f * perc - 1.70158f)) + start;
    }

    public static float LerpBackEaseOut(float start, float end, float perc)
    {
        return -(end - start) * ((1 - perc) * (1 - perc) * (2.70158f * (1 - perc) - 1.70158f) - 1f) + start;
    }
}
