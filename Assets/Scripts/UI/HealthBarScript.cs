using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class HealthBarScript : MonoBehaviour
    {
        #region Variables

        public Slider Slider;
        
        #endregion

        #region Unity Callbacks
    
        #endregion

        #region Auxiliar Methods

        public void SetHealth(int health)
        {
            Slider.value = health;
        }

        public void SetMaxHealth(int maxHealth)
        {
            Slider.maxValue = maxHealth;
            Slider.value = maxHealth;
        }
    
        #endregion
    }
}
