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

        private void Awake()
        {
            Debug.Log("Objeto Persistente CRIADO no cenário: " + SceneManager.GetActiveScene());
        }
        
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
            Debug.Log("Salvo na cena " + SceneManager.GetActiveScene().name);
        }

        public void LoadPlayerStats()
        {
            GetCurrentSceneComponents();
            PlayerHealthManager.CurrentHealth = PlayerPreviousHp;
            PlayerHealthManager.MaxHealth = PlayerPreviousMaxHp;
            Debug.Log("Carregado na " + SceneManager.GetActiveScene().name);   
        }

        void GetCurrentSceneComponents()
        {
            // Player Health Manager
            PlayerHealthManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealthManager>();
            Debug.Log("Get Components na cena " + SceneManager.GetActiveScene().name + "VALORES DA CENA: HP: " +
                      PlayerHealthManager.CurrentHealth + " MAX HP: " + PlayerHealthManager.MaxHealth);
        }

        public void Reset()
        {
            PlayerPreviousHp = 100;
            PlayerPreviousMaxHp = 100;
        }
        
    }
}
