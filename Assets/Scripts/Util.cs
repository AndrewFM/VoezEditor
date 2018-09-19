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

    public static bool EqualsPrecision(float a, float b, float prec)
    {
        return Mathf.Abs(a - b) <= prec;
    }

    // Easing Functions
    public static float LerpLinearEase(float start, float end, float perc)
    {
        return (end - start) * perc + start;
    }

    // Quadratic Easings
    public static float LerpQuadEaseIn(float start, float end, float perc)
    {
        float x = (perc * perc);
        return (end - start) * x + start;
    }

    public static float LerpQuadEaseOut(float start, float end, float perc)
    {
        float x = -((1f - perc) * (1f - perc)) + 1f;
        return (end - start) * x + start;
    }

    public static float LerpQuadEaseInOut(float start, float end, float perc)
    {
        float x = 0;
        if (perc < 0.5f)
            x = perc * perc * 2f;
        else
            x = (-1f + (4f-2f*perc) * perc);
        return (end - start) * x + start;
    }

    public static float LerpQuadEaseOutIn(float start, float end, float perc)
    {
        float x = 0;
        if (perc < 0.5f)
            x = Mathf.Sqrt(perc/2f);
        else
            x = 1f - (Mathf.Sqrt(1-perc)/Mathf.Sqrt(2f));
        return (end - start) * x + start;
    }

    // Circular Easings
    public static float LerpCircEaseIn(float start, float end, float perc)
    {
        float x = 1f - Mathf.Sqrt(1f - perc * perc);
        return (end - start) * x + start;
    }

    public static float LerpCircEaseOut(float start, float end, float perc)
    {
        float x = -(1f - Mathf.Sqrt(1f - (1f-perc) * (1f-perc))) + 1f;
        return (end - start) * x  + start;
    }

    public static float LerpCircEaseInOut(float start, float end, float perc)
    {
        float x = 0;
        if (perc < 0.5f)
            x = -0.5f * (Mathf.Sqrt(1f-4f*perc*perc) - 1f);
        else
            x = 0.5f * (Mathf.Sqrt(1f-(2f*perc-2f)*(2f*perc-2f)) + 1f);
        return (end - start) * x + start;
    }

    public static float LerpCircEaseOutIn(float start, float end, float perc)
    {
        float x = 0;
        if (perc < 0.5f)
            x = Mathf.Sqrt(perc-perc*perc);
        else
            x = 1f - Mathf.Sqrt(perc-perc*perc);
        return (end - start) * x + start;
    }

    // Exponential Easings
    public static float LerpExpEaseIn(float start, float end, float perc)
    {
        float x = Mathf.Pow(2, 10f * (perc - 1));
        return (end - start) * x + start;
    }

    public static float LerpExpEaseOut(float start, float end, float perc)
    {
        float x = -Mathf.Pow(2, 10f * -perc) + 1f;
        return (end - start) * x + start;
    }

    public static float LerpExpEaseInOut(float start, float end, float perc)
    {
        float x = 0;
        if (perc < 0.5f)
            x = 0.5f * Mathf.Pow(2f, 10f * (2f * perc - 1f));
        else
            x = 0.5f * -Mathf.Pow(2f, -10f * (2f * perc - 1f)) + 1f;
        return (end - start) * x + start;
    }

    public static float LerpExpEaseOutIn(float start, float end, float perc)
    {
        float x = 0;
        if (perc <= 0f)
            x = 0;
        else if (perc >= 1f)
            x = 1;
        else if (perc < 0.5f)
            x = Mathf.Log(2048f * perc) / (20f * Mathf.Log(2f));
        else
            x = Mathf.Log(-512 / (perc-1f)) / (20f * Mathf.Log(2f));
        return (end - start) * x + start;
    }

    // Back Easings
    public static float LerpBackEaseIn(float start, float end, float perc)
    {
        float x = perc * perc * (2.70158f * perc - 1.70158f);
        return (end - start) * x + start;
    }

    public static float LerpBackEaseOut(float start, float end, float perc)
    {
        float x = -((1f - perc) * (1f - perc) * (2.70158f * (1 - perc) - 1.70158f)) + 1f;
        return (end - start) * x + start;
    }

    public static float LerpBackEaseInOut(float start, float end, float perc)
    {
        float x = 0;
        if (perc < 0.5f)
            x = 2f * perc * perc * (7.189819f * perc - 2.5949095f);
        else
            x = 0.5f * ((2f * perc - 2f) * (2f * perc - 2f) * (3.5949095f * (2f * perc - 2f) + 2.5949095f) + 2f);
        return (end - start) * x + start;
    }

    public static float LerpBackEaseOutIn(float start, float end, float perc)
    {
        // TODO: Not correct formulas
        float x = 0;
        if (perc >= 0.5f)
            x = 2f * (perc - 0.5f) * (perc - 0.5f) * (7.189819f * (perc - 0.5f) - 2.5949095f) + 0.5f;
        else
            x = 0.5f * ((2f * (perc + 0.5f) - 2f) * (2f * (perc + 0.5f) - 2f) * (3.5949095f * (2f * (perc + 0.5f) - 2f) + 2.5949095f) + 2f) - 0.5f;
        return (end - start) * x + start;
    }

    // Elastic Easings
    public static float LerpElasticEaseIn(float start, float end, float perc)
    {
        float x = -(Mathf.Pow(2f, 10f * (perc - 1f)) * Mathf.Sin((perc - 1.1f) * 2f * Mathf.PI / 0.4f));
        return (end - start) * x + start;
    }

    public static float LerpElasticEaseOut(float start, float end, float perc)
    {
        float x = Mathf.Pow(2f,-10f * perc) * Mathf.Sin((perc - 0.1f) * 2f * Mathf.PI / 0.4f) + 1f;
        return (end - start) * x + start;
    }

    public static float LerpElasticEaseInOut(float start, float end, float perc)
    {
        float x = 0;
        if (perc < 0.5f)
            x = -0.5f * Mathf.Pow(2f, 10f * (2f * perc - 1f)) * Mathf.Sin(((2f * perc - 1f) - 0.1f) * 2f * Mathf.PI / 0.4f);
        else
            x = 0.5f * Mathf.Pow(2f, -10f * (2f * perc - 1f)) * Mathf.Sin(((2f * perc - 1f) - 0.1f) * 2f * Mathf.PI / 0.4f) + 1f;
        return (end - start) * x + start;
    }

    public static float LerpElasticEaseOutIn(float start, float end, float perc)
    {
        // TODO: Not correct formulas
        float x = 0;
        if (perc >= 0.5f)
            x = (-0.5f * Mathf.Pow(2f, 10f * (2f * (perc-0.5f) - 1f)) * Mathf.Sin(((2f * (perc-0.5f) - 1f) - 0.1f) * 2f * Mathf.PI / 0.4f)) + 0.5f;
        else
            x = (0.5f * Mathf.Pow(2f, -10f * (2f * (perc+0.5f) - 1f)) * Mathf.Sin(((2f * (perc+0.5f) - 1f) - 0.1f) * 2f * Mathf.PI / 0.4f) + 1f) - 0.5f;
        return (end - start) * x + start;
    }
}
