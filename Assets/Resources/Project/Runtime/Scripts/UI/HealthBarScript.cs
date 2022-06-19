using Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class HealthBarScript : MonoBehaviour
    {
        private void Start()
        {
            // Observe player health change event
            PlayerHealthManager.HealthChanged += OnHealthChanged;
        }

        [SerializeField] private Slider Slider;
        [SerializeField] private TextMeshProUGUI Amount;

        public void OnHealthChanged(float current, float max)
        {
            Slider.value = current;
            Amount.SetText(current + "/" + max);
        }
        

    }
}
