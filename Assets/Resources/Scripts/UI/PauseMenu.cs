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
      
        // Update is called once per frame
        void Update()
        {
            // Player can't pause while in level up menu
            if (LevelUpMenu.IsLevelingUp) return;
            
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                if (LevelManager.GameIsPaused)
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
            Time.timeScale = 1f;
            LevelManager.GameIsPaused = false;
            PauseMenuUI.SetActive(false);
        }

        public void LoadMenu()
        {
            Debug.Log("UÉ");
            Resume();
            SceneManager.LoadScene(0);
        }

        public void QuitGame()
        {
            Application.Quit();
        }

        private void Pause()
        {
            PauseMenuUI.SetActive(true);
            Time.timeScale = 0f;
            LevelManager.GameIsPaused = true;
        }
    
    }
}
