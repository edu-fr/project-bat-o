using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class HealthBarScript : MonoBehaviour
    {
        public Slider Slider;
        
        public void SetHealth(int health)
        {
            Slider.value = health;
        }

        public void SetMaxHealth(int maxHealth)
        {
            Slider.maxValue = maxHealth;
        }
    }
}
