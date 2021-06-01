using UnityEngine;

namespace Game
{
    public class TimeManager : MonoBehaviour
    {
        private void Start()
        {
        
        }

        private void Update()
        {
           
        }

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
