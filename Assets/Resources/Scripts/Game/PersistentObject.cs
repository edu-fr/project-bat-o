using System;
using Player;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
    public class PersistentObject : MonoBehaviour
    {
        private int Id;
        private PlayerHealthManager PlayerHealthManager;
        private HealthBarScript HealthBarScript;
        
        // Player Stats

        public int PlayerPreviousHp;
        public int PlayerPreviousMaxHp;

        private void Start()
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

        public void Reset()
        {
            PlayerPreviousHp = 100;
            PlayerPreviousMaxHp = 100;
        }
        
    }
}
