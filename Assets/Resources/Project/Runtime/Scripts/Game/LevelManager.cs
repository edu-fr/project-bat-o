using Resources.Project.Runtime.Scripts.Player;
using Resources.Project.Runtime.Scripts.UI;
using UnityEngine;

namespace Resources.Project.Runtime.Scripts.Game
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private LevelBuilder levelBuilder;
        [SerializeField] private Transform playerTransform;
        [SerializeField] private Transform playerSpawn;

        private RoadblockController RoadblockController;
        public int enemiesRemaining;
        
        [SerializeField] GameOverMenu GameOverMenu;
        private PlayerHealthManager _playerHealthManager;
        public static bool GameIsPaused = false;

        private void Awake()
        {
            _playerHealthManager = playerTransform.GetComponent<PlayerHealthManager>();
            PlayerHealthManager.HealthChanged += OnHealthChanged;
        }

        private void Start()
        {
            levelBuilder.BuildWorld();
        }

        private void Update()
        {
            if (GameIsPaused) return;

            if (enemiesRemaining == 0 && RoadblockController.IsBlocking)
            {
                RoadblockController.IsBlocking = false;
                // Highlight the path
            }
        }

        private void OnHealthChanged(float current, float max)
        {
            if (!(current <= 0)) return; // Return if player is still alive
            AudioManager.instance.Play("Player dying");
            GameIsPaused = true;
            GameOverMenu.OpenGameOverMenu();
            // Unsubscribe
            PlayerHealthManager.HealthChanged -= OnHealthChanged;
        }

    }
}

