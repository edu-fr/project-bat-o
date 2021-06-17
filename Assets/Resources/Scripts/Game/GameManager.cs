using System;
using Player;
using Resources.Scripts.UI;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
    public class GameManager : MonoBehaviour
    {
        // Necessary objects and prefabs
        private GameObject Player;
        public Transform PlayerPrefab;
        private Vector2 LevelStartingPoint;

        // Static variables 
        public static bool GameIsPaused = false;
        
        // Controllers
        private GameObject MenusCanvas;
        private GameOverMenu GameOverMenu;
        private PauseMenu PauseMenu;
        private PlayerHealthManager PlayerHealthManager;
        private RoadblockController RoadblockController;
        
        public int EnemiesRemaining { get; set; }
        
        private void Awake()
        {
            // Instantiating player
            while (!Player)
            {
                try
                {
                    LevelStartingPoint = GameObject.FindGameObjectWithTag("StartingPoint").transform.position;
                    Player = GameObject.FindGameObjectWithTag("Player");
                    if (!Player) Player = Instantiate(PlayerPrefab, LevelStartingPoint, Quaternion.identity).gameObject;
                    else Player.transform.position = LevelStartingPoint;
                    DontDestroyOnLoad(Player);
                }
                catch (NullReferenceException)
                {
                    Debug.LogError("Couldn't find level starting point. Can't instantiate new player.");
                }
            }

            RoadblockController = GameObject.FindGameObjectWithTag("Roadblock").GetComponent<RoadblockController>();
            if (!RoadblockController) Debug.LogError("Can't find RoadBlockController.");
            
            //AudioManager.instance.Play("Phase 1 background music");
            EnemiesRemaining = GameObject.FindGameObjectsWithTag("Enemy").Length;
            
            // Menus
            MenusCanvas = GameObject.FindGameObjectWithTag("MenusCanvas");
            PauseMenu = MenusCanvas.GetComponent<PauseMenu>();
            GameOverMenu = MenusCanvas.GetComponent<GameOverMenu>();

            PlayerHealthManager = Player.GetComponent<PlayerHealthManager>();
        }

        private void Start()
        {
            PauseMenu.PauseMenuUI.SetActive(false);
            GameOverMenu.GameOverMenuUI.SetActive(false);
            GameIsPaused = false; 
            Time.timeScale = 1f; 
        }
        
        // Update is called once per frame
        private void Update()
        {
            if(PlayerHealthManager.CurrentHealth <= 0) // Player died
            {
                AudioManager.instance.Play("Player dying");
                // Call game over menu
                GameOverMenu.OpenGameOverMenu();
            }

            if (EnemiesRemaining == 0 && RoadblockController.IsBlocking)
            {
                // Make arrow appear 
                RoadblockController.IsBlocking = false;
            }
        }
        
        public void GoToNextLevel()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}

