using Game;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class GameOverMenu : MonoBehaviour
    { 
        public CanvasRenderer GameOverMenuUI;

        public void OpenGameOverMenu()
        {
            if (GameOverMenuUI.gameObject.activeSelf) return;
            LevelManager.GameIsPaused = true;
            Time.timeScale = 0f;
            GameOverMenuUI.gameObject.SetActive(true);
        }
    
        public void TryAgain()
        {
            LevelManager.GameIsPaused = false;
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
            GameOverMenuUI.gameObject.SetActive(false);
            Time.timeScale = 1f;
        }

        public void LoadMainMenu()
        {
            GameOverMenuUI.gameObject.SetActive(false);
            LevelManager.GameIsPaused = false;
            SceneManager.LoadSceneAsync("Menu");
            Time.timeScale = 1f;
        }
    }
}
