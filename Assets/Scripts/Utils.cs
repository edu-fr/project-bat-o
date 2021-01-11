using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Debug.Log of path = AstarPath.cs line 835 & 837
// Debug.Log of scanning = AstarPath.cs line 1779

/*
     * Various assorted utilities functions
     * */
public static class UtilitiesClass
{

    public static Vector3 GetVectorFromAngle(float angle)
    {
        // angle = 0 -> 360
        float angleRad = angle * (Mathf.PI / 180f);
        return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
    }


    public static float GetAngleFromVectorFloat(Vector3 dir)
    {
        dir = dir.normalized;
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;

        return n;
    }

    public static Vector3 GetDirectionFromAngle(float angle)
    {
        if (angle >= 315 || angle < 45) return new Vector3(1, 0);
        if (angle >= 45 && angle < 135) return new Vector3(0, 1);
        if (angle >= 135 && angle < 225) return new Vector3(-1, 0);
        return new Vector3(0, -1);

    }

    public static Vector3 Get8DirectionFromAngle(float angle)
    {
        if (angle >= 337 || angle < 022) return new Vector3(1, 0);   // Right
        if (angle >= 022 && angle < 067) return new Vector3(1, 1);   // Right + Up
        if (angle >= 067 && angle < 112) return new Vector3(0, 1);   // Up
        if (angle >= 112 && angle < 157) return new Vector3(-1, 1);  // Left + Up
        if (angle >= 157 && angle < 202) return new Vector3(-1, 0);  // Left
        if (angle >= 202 && angle < 247) return new Vector3(-1, -1); // Left + Down
        if (angle >= 247 && angle < 292) return new Vector3(0, -1);  // Down
        if (angle >= 292 && angle < 337) return new Vector3(1, -1);  // Right + Down
        return new Vector3(0, 0);
    }
    
    public static Vector3 CustomLerp(Vector3 start, Vector3 end, float timeStartedLerping, float lerpTime)
    {
        var timeSinceStarted = Time.unscaledTime - timeStartedLerping;
        var percentageComplete= timeSinceStarted / lerpTime;
        var result = Vector3.Lerp(start, end, percentageComplete);
        return result;
    }

      
    // Returns 00-FF, value 0->255
    public static string Dec_to_Hex(int value) {
        return value.ToString("X2");
    }

    // Returns 0-255
    public static int Hex_to_Dec(string hex) {
        return Convert.ToInt32(hex, 16);
    }
        
    // Returns a hex string based on a number between 0->1
    public static string Dec01_to_Hex(float value) {
        return Dec_to_Hex((int)Mathf.Round(value*255f));
    }

    // Returns a float between 0->1
    public static float Hex_to_Dec01(string hex) {
        return Hex_to_Dec(hex)/255f;
    }
    
    // Get Hex Color FF00FF
    public static string GetStringFromColor(Color color) {
        string red = Dec01_to_Hex(color.r);
        string green = Dec01_to_Hex(color.g);
        string blue = Dec01_to_Hex(color.b);
        return red+green+blue;
    }
    
    // Get Hex Color FF00FFAA
    public static string GetStringFromColorWithAlpha(Color color) {
        string alpha = Dec01_to_Hex(color.a);
        return GetStringFromColor(color)+alpha;
    }

    // Sets out values to Hex String 'FF'
    public static void GetStringFromColor(Color color, out string red, out string green, out string blue, out string alpha) {
        red = Dec01_to_Hex(color.r);
        green = Dec01_to_Hex(color.g);
        blue = Dec01_to_Hex(color.b);
        alpha = Dec01_to_Hex(color.a);
    }
    
    // Get Hex Color FF00FF
    public static string GetStringFromColor(float r, float g, float b) {
        string red = Dec01_to_Hex(r);
        string green = Dec01_to_Hex(g);
        string blue = Dec01_to_Hex(b);
        return red+green+blue;
    }
    
    // Get Hex Color FF00FFAA
    public static string GetStringFromColor(float r, float g, float b, float a) {
        string alpha = Dec01_to_Hex(a);
        return GetStringFromColor(r,g,b)+alpha;
    }
    
    // Get Color from Hex string FF00FFAA
    public static Color GetColorFromString(string color) {
        float red = Hex_to_Dec01(color.Substring(0,2));
        float green = Hex_to_Dec01(color.Substring(2,2));
        float blue = Hex_to_Dec01(color.Substring(4,2));
        float alpha = 1f;
        if (color.Length >= 8) {
            // Color string contains alpha
            alpha = Hex_to_Dec01(color.Substring(6,2));
        }
        return new Color(red, green, blue, alpha);
    }

}