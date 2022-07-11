using System;
using Resources.Project.Runtime.Scripts.Game;
using UnityEngine;

namespace Resources.Project.Runtime.Scripts.UI
{
    public class MainMenu : MonoBehaviour
    {
        public void PlayGame()
        {
            throw new NotImplementedException();
        }

        public void QuitGame()
        {
            Debug.Log("Quit");
            Application.Quit();
        }
    }
}
