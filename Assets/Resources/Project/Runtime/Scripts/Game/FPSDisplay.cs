using UnityEngine;
using System.Collections;
 
public class FPSDisplay : MonoBehaviour
{
    float deltaTime = 0.0f;
    private float minFPS = 1000f;
    
    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
    }
 
    void OnGUI()
    {
        int w = Screen.width, h = Screen.height;
 
        GUIStyle style = new GUIStyle();
 
        Rect rect = new Rect(0, 0, w, h * 2 / 100);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / 100;
        style.normal.textColor = new Color (0.0f, 0.0f, 0.5f, 1.0f);
        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;
        if (fps < minFPS)
        {
            minFPS = fps;
            StartCoroutine(ResetMin());
        }
        string text = string.Format("{0:0.0} ms ({1:0.} fps). Min: {2:0.}", msec, fps, minFPS);
        GUI.Label(rect, text, style);
    }

    IEnumerator ResetMin()
    { 
        yield return new WaitForSeconds(3);
        minFPS = 9999f;
    }
}