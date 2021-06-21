using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
