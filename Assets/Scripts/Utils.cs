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
public static class Utilities
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
        else if (angle >= 45 && angle < 135) return new Vector3(0, 1);
        else if (angle >= 135 && angle < 225) return new Vector3(-1, 0);
        else /*if (angle >= 225 && angle < 315)*/ return new Vector3(0, -1);

    }
}