using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

namespace Resources.Scripts.UI
{
    public class LevelUpMenu : MonoBehaviour
    {
        public GameObject LevelUpUI;
        
        public TextMeshPro[] ButtonsTitle;
        public TextMeshPro[] ButtonsText;
        
        public static bool IsLevelingUp;
        private int OptionsAmount;
        
        void Start()
        {
            
        }

        void Update()
        {
        
        }

        public void OpenLevelUpMenu()
        {
            List<int> options = GetOptions();
            for (int i = 0; i < ButtonsTitle.Length; i++)
            {
                ConfigureButton(ButtonsTitle[i], ButtonsText[i], options[i]);
            }
            LevelUpUI.SetActive(true);
            Time.timeScale = 0f;
            IsLevelingUp = true;
        }

        private void CloseLevelUpMenu()
        {
            LevelUpUI.SetActive(false);
            Time.timeScale = 1f;
            IsLevelingUp = false;
        }

        private List<int> GetOptions()
        {
            var options = new List<int>();
            // First option (Effect)
            var firstOption = new Random().Next(UpgradesDatabase.EffectsBottomIndex, UpgradesDatabase.EffectsTopIndex);
            options.Add(firstOption);
            
            // Second option (Effect)
            int secondOption;
            var safetyCounter = 0;
            do
            {
                secondOption = new Random().Next(UpgradesDatabase.EffectsBottomIndex, UpgradesDatabase.EffectsTopIndex);
                safetyCounter++;
                if (safetyCounter > 1000) break;
            } while (firstOption == secondOption);
            options.Add(secondOption);
            
            // Third option (Mechanical)
            var thirdOption = new Random().Next(UpgradesDatabase.MechanicalBottomIndex, UpgradesDatabase.MechanicalTopIndex);
            options.Add(thirdOption);
            
            // Fourth option (Stats)
            var fourthOption = new Random().Next(UpgradesDatabase.StatsBottomIndex, UpgradesDatabase.StatsTopIndex);
            options.Add(fourthOption);
            
            return options;
        }

        void ConfigureButton(TextMeshPro title, TextMeshPro text, int selectedPowerUp)
        {
            
            title.SetText(GetTitle());
        }

        void GetPowerUpInfo()
        {
            
        }
        
        string GetTitle()
        {
            return "oi";
        }

        string GetText()
        {
            return "tchau";
        }
    }
    
}
