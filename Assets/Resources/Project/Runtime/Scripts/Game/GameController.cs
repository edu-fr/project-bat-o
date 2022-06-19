using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public Transform PlayerPrefab;
    private static Transform _playerPrefab;

    private void Awake()
    {
        _playerPrefab = PlayerPrefab;
    }
    
    public static void InstantiateNewPlayer()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        Destroy(player);
        var preloadReference = GameObject.FindGameObjectWithTag("Preload").transform;
        Instantiate(_playerPrefab.gameObject, Vector3.zero, Quaternion.identity, preloadReference);
    }

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
