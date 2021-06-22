using System.Collections;
using System.Collections.Generic;
using Game;
using Resources.Scripts.UI;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{ 
    public GameObject GameOverMenuUI;
    private GameController GameController;
    
    private void Awake()
    {
        GameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }
    
    public void OpenGameOverMenu()
    {
        if (GameOverMenuUI.activeSelf) return;
        LevelManager.GameIsPaused = true;
        Time.timeScale = 0f;
        GameOverMenuUI.SetActive(true);
    }
    
    public void TryAgain()
    {
        GameController.InstantiateNewPlayer();
        LevelManager.GameIsPaused = false;
        GameController.LoadSceneAsynchronously(1);
        GameOverMenuUI.SetActive(false);
        Time.timeScale = 1f;
    }

    public void LoadMenu()
    {
        GameOverMenuUI.SetActive(false);
        LevelManager.GameIsPaused = false;
        GameController.LoadSceneAsynchronously(0);
        Time.timeScale = 1f;
    }
}
