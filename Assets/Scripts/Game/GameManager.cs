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

        #endregion

        #region UnityCallbacks

        // Start is called before the first frame update
        private void Start()
        {
            Player = GameObject.FindGameObjectWithTag("Player");
            PlayerHealthManager = Player.GetComponent<PlayerHealthManager>();
            //AudioManager.instance.Play("Phase 1 background music");
        }

        // Update is called once per frame
        private void Update()
        {
            if(PlayerHealthManager.GetCurrentHp() <= 0)
            {
                AudioManager.instance.Play("Player dying");
                SceneManager.LoadScene(0);
            }
        }
        #endregion

        #region Auxiliar methods

        #endregion
    }
}

