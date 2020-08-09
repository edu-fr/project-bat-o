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
            Debug.Log("Saved on " + SceneManager.GetActiveScene().name);
        }

        public void LoadPlayerStats()
        {
            GetCurrentSceneComponents();
            PlayerHealthManager.CurrentHealth = PlayerPreviousHp;
            PlayerHealthManager.MaxHealth = PlayerPreviousMaxHp;
            Debug.Log("Loaded on " + SceneManager.GetActiveScene().name);   
        }

        void GetCurrentSceneComponents()
        {
            // Player Health Manager
            PlayerHealthManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealthManager>();
            Debug.Log("Get Components on " + SceneManager.GetActiveScene().name);
        }

        public void Reset()
        {
            PlayerPreviousHp = 0;
            PlayerPreviousMaxHp = 0;
        }
        
    }
}
