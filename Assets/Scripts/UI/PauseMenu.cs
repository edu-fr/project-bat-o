using Game;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class PauseMenu : MonoBehaviour
    {
        public static bool GameIsPaused = false;
        public GameObject PauseMenuUI;
        private PersistentObject PersistentObject;

        private void Start()
        {
            // Only call at Start cause Game Manager create the persistent instance on Awake
            PersistentObject = GameObject.FindGameObjectWithTag("Persistent").GetComponent<PersistentObject>();
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                if (GameIsPaused)
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
            GameIsPaused = false;
        }

        public void LoadMenu()
        {
            PersistentObject.Reset();
            Debug.Log("Objeto Persistente Resetado no cenário: " + SceneManager.GetActiveScene().name + ". Valores atuais: HP: " + PersistentObject.PlayerPreviousHp +
                      " MAX HP: " + PersistentObject.PlayerPreviousMaxHp);
            SceneManager.LoadScene(0);
            Resume();
        }

        public void QuitGame()
        {
            Application.Quit();
        }

        private void Pause()
        {
            PauseMenuUI.SetActive(true);
            Time.timeScale = 0f;
            GameIsPaused = true;
        }
    
    }
}
