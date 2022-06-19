using Game;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class PauseMenu : MonoBehaviour
    {
        public GameObject PauseMenuUI;

        public void Resume()
        {
            Time.timeScale = 1f;
            LevelManager.GameIsPaused = false;
            PauseMenuUI.SetActive(false);
        }

        public void LoadMenu()
        {
            Resume();
            SceneManager.LoadScene(0);
        }

        public void QuitGame()
        {
            Application.Quit();
        }

        public void Pause()
        {
            PauseMenuUI.SetActive(true);
            
            Time.timeScale = 0f;
            LevelManager.GameIsPaused = true;
        }
    
    }
}
