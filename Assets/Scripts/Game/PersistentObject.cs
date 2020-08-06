using System;
using Player;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
    public class PersistentObject : MonoBehaviour
    {
        private PlayerHealthManager PlayerHealthManager;
        private HealthBarScript HealthBarScript;
        
        // Player Stats
        [NonSerialized]
        public int PlayerPreviousHp;
        [NonSerialized]
        public int PlayerPreviousMaxHp;

        private void Awake()
        {
            GameObject[] persistentObjects = GameObject.FindGameObjectsWithTag("Persistent");
            if (persistentObjects.Length > 1)
            {
                Destroy(this.gameObject);
            }
        }
        
        private  void Start()
        {
            DontDestroyOnLoad(this.gameObject);
            PlayerHealthManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealthManager>();
            HealthBarScript = GameObject.FindGameObjectWithTag("HealthBar").GetComponent<HealthBarScript>();
        }

        public void SavePlayerStats()
        {
            PlayerPreviousHp = PlayerHealthManager.CurrentHealth;
            PlayerPreviousMaxHp = PlayerHealthManager.MaxHealth;
        }

        public void LoadPlayerStats()
        {
            GetCurrentSceneComponents();
            PlayerHealthManager.CurrentHealth = PlayerPreviousHp;
            PlayerHealthManager.MaxHealth = PlayerPreviousMaxHp;
            
        }

        void GetCurrentSceneComponents()
        {
            // Player Health Manager
            PlayerHealthManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealthManager>();
        }
    }
}
