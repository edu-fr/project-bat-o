using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;
using UDPU = Resources.Scripts.UI.UpgradesDatabase.PowerUps;
using UD = Resources.Scripts.UI.UpgradesDatabase;

namespace Resources.Scripts.UI
{
    public class LevelUpMenu : MonoBehaviour
    {
        public GameObject LevelUpUI;
        
        public TextMeshPro[] ButtonsTitle;
        public TextMeshPro[] ButtonsText;
        
        public static bool IsLevelingUp;
        private int OptionsAmount;

        public PowerUpController PlayerPowerUpController;
        
        void Awake()
        {
            PlayerPowerUpController = GameObject.FindGameObjectWithTag("Player").GetComponent<PowerUpController>();
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

        void ConfigureButton(TextMeshPro title, TextMeshPro text, UDPU selectedPowerUp)
        {
            title.SetText(GetEffectPowerUpText(selectedPowerUp, GetEffectPowerUpInfo(selectedPowerUp)));
            text.SetText(GetEffectPowerUpTitle(selectedPowerUp, GetEffectPowerUpInfo(selectedPowerUp)));
        }

        int GetEffectPowerUpInfo(UDPU powerUp)
        {
            return powerUp switch
            {
                UDPU.FireAttack => PlayerPowerUpController.FireLevel,
                UDPU.ElectricAttack => PlayerPowerUpController.ElectricLevel,
                UDPU.IceAttack => PlayerPowerUpController.IceLevel,
                _ => 0
            };
        }
        
        int GetMechanicalPowerUpInfo(UDPU powerUp)
        {
            return powerUp switch
            {
                UDPU.PerfectDodgeAttack => /* PlayerPowerUpController... */ 1,
                _ => -1
            };
        }

        string GetEffectPowerUpText(UDPU powerUp, int level)
        {
            return powerUp switch
            {
                UDPU.ElectricAttack => UD.ElectricAttackTexts[level],
                UDPU.FireAttack => UD.FireAttackTexts[level],
                UDPU.IceAttack => UD.IceAttackTexts[level],
                _ => "Error: Attack text not found."
            };
        }

        string GetEffectPowerUpTitle(UDPU powerUp, int level)
        {
            return powerUp switch
            {
                UDPU.ElectricAttack => UD.ElectricAttackTitles[level - 1],
                UDPU.FireAttack => UD.FireAttackTitles[level - 1],
                UDPU.IceAttack => UD.IceAttackTitles[level - 1],
                _ => "Error: Attack title not found."
            };
        }

    }
    
}
