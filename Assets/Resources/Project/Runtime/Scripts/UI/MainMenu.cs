using UnityEngine;

public class MainMenu : MonoBehaviour
{
    private GameController GameController;

    private void Awake()
    {
        GameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }
    public void PlayGame()
    {
        GameController.InstantiateNewPlayer();
        GameController.LoadSceneAsynchronously(1);
    }

    public void QuitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}
