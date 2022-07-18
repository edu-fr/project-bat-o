using Game;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class PauseMenu : MonoBehaviour
    {
        public CanvasRenderer pauseMenuUI;

        public void Resume()
        {
            Time.timeScale = 1f;
            LevelManager.GameIsPaused = false;
            pauseMenuUI.gameObject.SetActive(false);
        }

        public void LoadPauseMenu()
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
            pauseMenuUI.gameObject.SetActive(true);
            Time.timeScale = 0f;
            LevelManager.GameIsPaused = true;
        }
    
    }
}
