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
        if (angle >= 330 || angle < 30) return new Vector3(1, 0);    // Right
        if (angle >= 30 && angle < 60) return new Vector3(1, 1);     // Right + Up
        if (angle >= 60 && angle < 120) return new Vector3(0, 1);    // Up
        if (angle >= 120 && angle < 150) return new Vector3(-1, 1);  // Left + Up
        if (angle >= 150 && angle < 210) return new Vector3(-1, 0);  // Left
        if (angle >= 210 && angle < 240) return new Vector3(-1, -1); // Left + Down
        if (angle >= 240 && angle < 300) return new Vector3(0, -1);  // Down
        return new Vector3(0, -1);                                   // Right + Down
       
    }
    
    public static Vector3 CustomLerp(Vector3 start, Vector3 end, float timeStartLerping, float lerpTime)
    {
        var timeSinceStartLerping = Time.unscaledTime - timeStartLerping;
        var lerpingPercentage = timeSinceStartLerping / lerpTime;
        var result = Vector3.Lerp(start, end, lerpingPercentage);
        return result;
    }
}