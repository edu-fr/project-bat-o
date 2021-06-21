using System;
using System.Collections;
using Player;
using Resources.Scripts.UI;
using UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private string NextScene;
        
        private GameObject Player;
        private Vector2 LevelStartingPoint;
        public static bool GameIsPaused = false;
        
        // Controllers
        private GameObject MenusCanvas;
        private GameObject GameOverMenuUI;
        private PlayerHealthManager PlayerHealthManager;
        private RoadblockController RoadblockController;
        
        public int EnemiesRemaining { get; set; }

        private void Start()
        {
            Player = GameObject.FindGameObjectWithTag("Player");
            Player.transform.position = GameObject.FindGameObjectWithTag("StartingPoint").transform.position;
            PlayerHealthManager = Player.GetComponent<PlayerHealthManager>();
            
            MenusCanvas = GameObject.FindGameObjectWithTag("MenusCanvas");
            GameOverMenuUI = MenusCanvas.GetComponent<GameOverMenu>().GameOverMenuUI;
            
            RoadblockController = GameObject.FindGameObjectWithTag("Roadblock").GetComponent<RoadblockController>();
            // Configure AudioManager.instance.Play("Phase 1 background music");
            EnemiesRemaining = GameObject.FindGameObjectsWithTag("Enemy").Length;
        }

        // Update is called once per frame
        private void Update()
        {
            if (GameIsPaused) return;
            
            if(PlayerHealthManager.CurrentHealth <= 0) // Player died
            {
                AudioManager.instance.Play("Player dying");
                GameIsPaused = true;
                MenusCanvas.GetComponent<GameOverMenu>().OpenGameOverMenu();
            }

            if (EnemiesRemaining == 0 && RoadblockController.IsBlocking)
            {
                // Make arrow appear 
                RoadblockController.IsBlocking = false;
            }
        }
        
        public void GoToNextLevel()
        {
            SceneManager.LoadScene(NextScene);
        }
    }
}

