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
    public GameObject Player; 

    private void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
    }
    
    public void OpenGameOverMenu()
    {
        GameManager.GameIsPaused = true;
        Time.timeScale = 0f;
        GameOverMenuUI.SetActive(true);
    }
    
    public void TryAgain()
    {
        Destroy(Player);
        SceneManager.LoadScene(1);
    }

    public void LoadMenu()
    {
        Destroy(Player);
        SceneManager.LoadScene(0);
        Time.timeScale = 1f;
    }
}
