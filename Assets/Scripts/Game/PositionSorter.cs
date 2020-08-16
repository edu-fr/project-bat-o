using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Game
{
    [InitializeOnLoad]
    class SpriteSorter
    {
        static SpriteSorter()
        {
            Initialize();
        }
        [RuntimeInitializeOnLoadMethod]
        static void Initialize()
        {
            Debug.Log("Inicializou");
            GraphicsSettings.transparencySortMode = TransparencySortMode.CustomAxis;
            GraphicsSettings.transparencySortAxis = new Vector3(0.0f, 1.0f, 1.0f);
        }
    }
}