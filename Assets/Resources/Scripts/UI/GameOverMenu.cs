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
    
    public void OpenGameOverMenu()
    {
        LevelManager.GameIsPaused = true;
        Time.timeScale = 0f;
        GameOverMenuUI.SetActive(true);
    }
    
    public void TryAgain()
    {
        GameController.InstantiateNewPlayer();
        SceneManager.LoadScene(1);
        LevelManager.GameIsPaused = false;
        Time.timeScale = 1f;
        GameOverMenuUI.SetActive(false);
    }

    public void LoadMenu()
    {
        GameOverMenuUI.SetActive(false);
        LevelManager.GameIsPaused = false;
        SceneManager.LoadScene(0);
        Time.timeScale = 1f;
    }
}
