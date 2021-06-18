using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Preload : MonoBehaviour
{
    // loads before any other scene:
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    
    public static void LoadMain()
    {
        GameObject main = Instantiate(UnityEngine.Resources.Load("Preload")) as GameObject;
        DontDestroyOnLoad(main);
    }
}
