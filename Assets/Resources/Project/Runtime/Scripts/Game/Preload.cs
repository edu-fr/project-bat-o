using UnityEngine;

namespace Resources.Project.Runtime.Scripts.Game
{
    public class Preload : MonoBehaviour
    {
        // loads before any other scene:
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    
        public static void LoadMain()
        {
            GameObject main = Instantiate(UnityEngine.Resources.Load("Project/Runtime/Prefabs/Game/Preload")) as GameObject;
            DontDestroyOnLoad(main);
        }
    }
}
