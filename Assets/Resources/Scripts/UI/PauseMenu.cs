using System;
using Game;
using Resources.Scripts.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class PauseMenu : MonoBehaviour
    {
        public GameObject PauseMenuUI;
        public GameObject Player;

        private void Start()
        {
            Player = GameObject.FindGameObjectWithTag("Player");
        }

        // Update is called once per frame
        void Update()
        {
            // Player can't pause while in level up menu
            if (LevelUpMenu.IsLevelingUp) return;
            
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                if (GameManager.GameIsPaused)
                {
                    Resume();
                }
                else
                {
                    Pause();
                }
            }
        }
        
        public void Resume()
        {
            PauseMenuUI.SetActive(false);
            Time.timeScale = 1f;
            GameManager.GameIsPaused = false;
        }

        public void LoadMenu()
        {
            Destroy(Player);
            SceneManager.LoadScene(0);
            Resume();
        }

        public void QuitGame()
        {
            Destroy(Player);
            Application.Quit();
        }

        private void Pause()
        {
            PauseMenuUI.SetActive(true);
            Time.timeScale = 0f;
            GameManager.GameIsPaused = true;
        }
    
    }
}
