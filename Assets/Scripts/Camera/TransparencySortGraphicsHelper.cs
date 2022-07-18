
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

#if UNITY_EDITOR
namespace Camera
{
    [InitializeOnLoad]
#endif
    class TransparencySortGraphicsHelper
    {
        static TransparencySortGraphicsHelper()
        {
            OnLoad();
        }
 
        [RuntimeInitializeOnLoadMethod]
        static void OnLoad()
        {
            GraphicsSettings.transparencySortMode = TransparencySortMode.CustomAxis;
            GraphicsSettings.transparencySortAxis = new Vector3(0.0f, 1.0f, 1.0f);
        }
    }
}