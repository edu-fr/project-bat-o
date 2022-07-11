using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Resources.Project.Runtime.Scripts.Game
{
    public class GameController : MonoBehaviour
    {
        public void LoadSceneAsynchronously(int sceneIndex)
        {
            StartCoroutine(LoadAsyncSceneByIndex(sceneIndex));
        }
    
        public void LoadSceneAsynchronously(string sceneName)
        {
            StartCoroutine(LoadAsyncSceneByName(sceneName));
        }
    
        IEnumerator LoadAsyncSceneByIndex(int sceneIndex)
        {
            var asyncLoad = SceneManager.LoadSceneAsync(sceneIndex);
            while (!asyncLoad.isDone)
            {
                yield return null;
            }
        }
    
        IEnumerator LoadAsyncSceneByName(string sceneName)
        {
            var asyncLoad = SceneManager.LoadSceneAsync(sceneName);
            while (!asyncLoad.isDone)
            {
                yield return null;
            }
        }
    }
}
