using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region Variables
    private GameObject player;
    private HealthManager playerHealthManager;

    #endregion

    #region UnityCallbacks

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerHealthManager = player.GetComponent<HealthManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(playerHealthManager.getCurrentHP() <= 0)
        {
            SceneManager.LoadScene(0);
        }
    }
    #endregion

    #region Auxiliar methods

    #endregion
}

