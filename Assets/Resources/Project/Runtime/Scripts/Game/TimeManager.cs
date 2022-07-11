using UnityEngine;

namespace Resources.Project.Runtime.Scripts.Game
{
    public class TimeManager : MonoBehaviour
    {
        public static void ChangeTimeScale(float value)
        {
            Time.timeScale = Mathf.Clamp(value, 0, 1);
        }

        public static void BackTimeToStandardFlow()
        {
            Time.timeScale = 1;
        }
    }
}
