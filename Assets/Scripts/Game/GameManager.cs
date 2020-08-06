using Player;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
    public class GameManager : MonoBehaviour
    {
        #region Variables
        private GameObject Player;
        private PlayerHealthManager PlayerHealthManager;
        private RoadblockController RoadblockController;
        public PersistentObject PersistentObject { get; private set; }
        
        public PersistentObject PersistentPrefab;
        public int EnemiesRemaining { get; set; }

        #endregion

        #region UnityCallbacks

        // Start is called before the first frame update
        private void Start()
        {
            RoadblockController = GameObject.FindGameObjectWithTag("Roadblock").GetComponent<RoadblockController>();
            Player = GameObject.FindGameObjectWithTag("Player");
            PlayerHealthManager = Player.GetComponent<PlayerHealthManager>();
            //AudioManager.instance.Play("Phase 1 background music");
            
            EnemiesRemaining = GameObject.FindGameObjectsWithTag("Enemy").Length;

            PersistentObject = GameObject.FindGameObjectWithTag("Persistent").GetComponent<PersistentObject>();

            if (PersistentObject == null)
            {
                PersistentObject = Instantiate(PersistentPrefab);
            }
            // Set player's health accordly to previous level

            if (PersistentObject.PlayerPreviousHp != 0)
            {
                PersistentObject.LoadPlayerStats();
            }
        }

        // Update is called once per frame
        private void Update()
        {
            if(PlayerHealthManager.CurrentHealth <= 0) // Player died
            {
                AudioManager.instance.Play("Player dying");
                Destroy(PersistentObject);
                SceneManager.LoadScene(0);
                
            }

            if (EnemiesRemaining == 0 && RoadblockController.IsBlocking)
            {
                RoadblockController.IsBlocking = false;
            }
            
            


        }
        #endregion

        #region Auxiliar methods

        public void GoToNextLevel()
        {
            PersistentObject.SavePlayerStats();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        
        #endregion
    }
}

