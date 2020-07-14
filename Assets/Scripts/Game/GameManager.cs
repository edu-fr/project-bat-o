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
        void Start()
        {
            Player = GameObject.FindGameObjectWithTag("Player");
            PlayerHealthManager = Player.GetComponent<PlayerHealthManager>();
        }

        // Update is called once per frame
        void Update()
        {
            if(PlayerHealthManager.GetCurrentHp() <= 0)
            {
                SceneManager.LoadScene(0);
            }
        }
        #endregion

        #region Auxiliar methods

        #endregion
    }
}

