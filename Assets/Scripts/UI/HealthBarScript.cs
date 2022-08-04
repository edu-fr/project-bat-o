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
            PlayerHealthManager.HealthChanged += OnCurrentHPChanged;
        }

        [SerializeField] private Slider Slider;
        [SerializeField] private TextMeshProUGUI Amount;

        private void OnCurrentHPChanged(float current, float max)
        {
            Slider.value = current;
            Slider.maxValue = max;
            Amount.SetText(current + "/" + max);
        }
    }
}
