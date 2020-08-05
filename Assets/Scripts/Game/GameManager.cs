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
        private GameObject Roadblock;
        private int EnemiesRemaining { get; set; }

        #endregion

        #region UnityCallbacks

        // Start is called before the first frame update
        private void Start()
        {
            Roadblock = GameObject.FindGameObjectWithTag("Roadblock");
            Player = GameObject.FindGameObjectWithTag("Player");
            PlayerHealthManager = Player.GetComponent<PlayerHealthManager>();
            //AudioManager.instance.Play("Phase 1 background music");
            
            EnemiesRemaining = GameObject.FindGameObjectsWithTag("Enemy").Length;
        }

        // Update is called once per frame
        private void Update()
        {
            if(PlayerHealthManager.GetCurrentHp() <= 0)
            {
                AudioManager.instance.Play("Player dying");
                SceneManager.LoadScene(0);
            }
            
            Debug.Log(EnemiesRemaining.Length);
            
            
        }
        #endregion

        #region Auxiliar methods

        public void GoToNextLevel()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        
        #endregion
    }
}

