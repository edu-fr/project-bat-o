using Player;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
    public class GameManager : MonoBehaviour
    {
        private GameObject Player;
        private PlayerHealthManager PlayerHealthManager;
        private RoadblockController RoadblockController;
        
        public GameObject Persistent { get; private set; }
        public PersistentObject PersistentObject { get; private set; }
        
        public Transform PersistentPrefab;
        public int EnemiesRemaining { get; set; }
        
        private void Awake()
        {
            Persistent = GameObject.FindGameObjectWithTag("Persistent");

            if (Persistent == null)
            {
                Persistent = Instantiate(PersistentPrefab, null).gameObject;
            }
            PersistentObject = Persistent.GetComponent<PersistentObject>();
        }

        // Start is called before the first frame update
        private void Start()
        {
            RoadblockController = GameObject.FindGameObjectWithTag("Roadblock").GetComponent<RoadblockController>();
            Player = GameObject.FindGameObjectWithTag("Player");
            PlayerHealthManager = Player.GetComponent<PlayerHealthManager>();
            //AudioManager.instance.Play("Phase 1 background music");
            
            EnemiesRemaining = GameObject.FindGameObjectsWithTag("Enemy").Length;

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
                PersistentObject.Reset();
                SceneManager.LoadScene(0);
                
            }

            if (EnemiesRemaining == 0 && RoadblockController.IsBlocking)
            {
                RoadblockController.IsBlocking = false;
            }
            
            


        }
        
        public void GoToNextLevel()
        {
            PersistentObject.SavePlayerStats();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}

