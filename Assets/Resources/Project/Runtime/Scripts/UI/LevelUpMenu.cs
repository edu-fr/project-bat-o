using System;
using System.Collections.Generic;
using Player;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;
using UDPU = Resources.Scripts.UI.UpgradesDatabase.PowerUps;
using UD = Resources.Scripts.UI.UpgradesDatabase;

namespace Resources.Scripts.UI
{
    public class LevelUpMenu : MonoBehaviour
    {
        public GameObject LevelUpUI;
        public List<GameObject> TitleTextObjects;
        public List<GameObject> MainTextObjects;

        private List<TextMeshProUGUI> Titles;
        private List<TextMeshProUGUI> Texts;
        
        public static bool IsLevelingUp;
        private List<UDPU> Options;
        
        private PowerUpController PlayerPowerUpController;
        private PlayerStatsController PlayerStatsController;
        
        void Awake()
        {
            Titles = new List<TextMeshProUGUI>();
            Texts = new List<TextMeshProUGUI>();
           
            for (var i = 0; i < 4; i++)
            {
                Titles.Add(TitleTextObjects[i].GetComponent<TextMeshProUGUI>());
                Texts.Add(MainTextObjects[i].GetComponent<TextMeshProUGUI>());
            }
        }

        private void Update()
        {
            if (!PlayerStatsController || !PlayerStatsController)
            {
                var player = GameObject.FindGameObjectWithTag("Player");
                PlayerPowerUpController = player.GetComponent<PowerUpController>();
                PlayerStatsController = player.GetComponent<PlayerStatsController>();
            }
        }

        public void OpenLevelUpMenu()
        {
            Options = GetOptions();
            for (int i = 0; i < Titles.Count; i++)
            {
                ConfigureButton(Titles[i], Texts[i], Options[i]);
            } ;
            LevelUpUI.SetActive(true);
            Time.timeScale = 0f;
            IsLevelingUp = true;
        }

        public void FirstPowerUpClicked()
        {
            ApplyPowerUp(Options[0]);   
            CloseLevelUpMenu();
        }
        
        public void SecondPowerUpClicked()
        {
            ApplyPowerUp(Options[1]);   
            CloseLevelUpMenu();
        }
        
        public void ThirdPowerUpClicked()
        {
            ApplyPowerUp(Options[2]);  
            CloseLevelUpMenu();
        }

        public void FourthPowerUpClicked()
        {
            ApplyPowerUp(Options[3]);   
            CloseLevelUpMenu();
        }
        
        private void ApplyPowerUp(UDPU powerUp)
        {
            switch (powerUp)
            {
                // Stats
                case UDPU.AttackDamageUp:
                    PlayerPowerUpController.AttackLevel++;
                    PlayerStatsController.UpdatePhysicalDamage();
                    break;
                
                case UDPU.PhysicalDefenseUp:
                    PlayerPowerUpController.PhysicalDefenseLevel++;
                    PlayerStatsController.UpdatePhysicalDefense();
                    break;
                
                case UDPU.MagicalDefenseUp:
                    PlayerPowerUpController.MagicalDefenseLevel++;
                    PlayerStatsController.UpdateMagicalDefense();
                    break;

                case UDPU.HpUp:
                    PlayerPowerUpController.HpLevel++;
                    PlayerStatsController.UpdateMaxHp();
                    break;

                case UDPU.CriticalRateUp:
                    PlayerPowerUpController.CriticalRateLevel++;
                    PlayerStatsController.UpdateCriticalRate();
                    break;

                case UDPU.CriticalDamageUp:
                    PlayerPowerUpController.CriticalDamageLevel++;
                    PlayerStatsController.UpdateCriticalDamage();
                    break;
                
                // Effects
                case UDPU.FireAttack:
                    PlayerPowerUpController.FireLevel++;
                    PlayerStatsController.UpdateFireDamage();
                    PlayerStatsController.UpdateFireAttackRate();
                    break;

                case UDPU.ElectricAttack:
                    PlayerPowerUpController.ElectricLevel++;
                    PlayerStatsController.UpdateElectricalDamage();
                    PlayerStatsController.UpdateElectricAttackRate();
                    PlayerStatsController.UpdateElectricRange();
                    PlayerStatsController.UpdateParalyzeDuration();
                    break;

                case UDPU.IceAttack:
                    PlayerPowerUpController.IceLevel++;
                    PlayerStatsController.UpdateIceDamage();
                    PlayerStatsController.UpdateIceAttackRate();
                    PlayerStatsController.UpdateFrostDuration();
                    PlayerStatsController.UpdateShatterDamage();
                    break;

                case UDPU.LifeStealUp:
                    PlayerPowerUpController.LifeStealLevel++;
                    PlayerStatsController.UpdateLifeStealPercentage();
                    PlayerStatsController.UpdateLifeStealAttackRate();
                    break;
                
                // Mechanical
                case UDPU.PerfectDodgeAttack:
                    PlayerPowerUpController.PerfectDodgeLevel++;
                    break;

                case UDPU.HitProjectiles:
                    PlayerPowerUpController.HitProjectilesLevel++;
                    break;
            }
        }
        
        private void CloseLevelUpMenu()
        {
            LevelUpUI.SetActive(false);
            Time.timeScale = 1f;
            IsLevelingUp = false;
        }

        private List<UDPU> GetOptions()
        {
            var options = new List<UDPU>();
            // First option (Effect)
            Random.InitState((int) DateTime.Now.Ticks);
            var firstOption = Random.Range(UpgradesDatabase.EffectsBottomIndex, UpgradesDatabase.EffectsTopIndex + 1);
            options.Add((UDPU) firstOption);
            
            // Second option (Effect)
            int secondOption;
            do
            {
                Random.InitState((int) DateTime.Now.Ticks);
                secondOption = Random.Range(UpgradesDatabase.EffectsBottomIndex, UpgradesDatabase.EffectsTopIndex + 1);
            } while (firstOption == secondOption);
            options.Add((UDPU) secondOption);
            
            // Third option (Mechanical)
            Random.InitState((int) DateTime.Now.Ticks);
            var thirdOption = Random.Range(UpgradesDatabase.MechanicalBottomIndex, UpgradesDatabase.MechanicalTopIndex + 1);
            options.Add((UDPU) thirdOption);
            
            // Fourth option (Stats)
            Random.InitState((int) DateTime.Now.Ticks);
            var fourthOption = Random.Range(UpgradesDatabase.StatsBottomIndex, UpgradesDatabase.StatsTopIndex + 1);
            options.Add((UDPU) fourthOption);
            
            return options;
        }

        void ConfigureButton(TextMeshProUGUI title, TextMeshProUGUI text, UDPU selectedPowerUp)
        {
            var powerUpInfo = GetPowerUpInfo(selectedPowerUp);
            title.SetText(GetPowerUpTitle(selectedPowerUp, powerUpInfo));
            text.SetText(GetPowerUpText(selectedPowerUp, powerUpInfo));
        }

        int GetPowerUpInfo(UDPU powerUp)
        {
            return powerUp switch
            {
                // Stats
                UDPU.AttackDamageUp => PlayerPowerUpController.AttackLevel,
                UDPU.PhysicalDefenseUp => PlayerPowerUpController.PhysicalDefenseLevel,
                UDPU.MagicalDefenseUp => PlayerPowerUpController.PhysicalDefenseLevel,
                UDPU.HpUp => PlayerPowerUpController.HpLevel,
                UDPU.CriticalRateUp => PlayerPowerUpController.CriticalRateLevel,
                UDPU.CriticalDamageUp => PlayerPowerUpController.CriticalDamageLevel,
                
                // Effects
                UDPU.FireAttack => PlayerPowerUpController.FireLevel,
                UDPU.ElectricAttack => PlayerPowerUpController.ElectricLevel,
                UDPU.IceAttack => PlayerPowerUpController.IceLevel,
                UDPU.LifeStealUp => PlayerPowerUpController.LifeStealLevel,
                
                // Mechanical
                UDPU.PerfectDodgeAttack => PlayerPowerUpController.PerfectDodgeLevel,
                UDPU.HitProjectiles => PlayerPowerUpController.HitProjectilesLevel,
                
                _ => 0
            };
        }

        string GetPowerUpText(UDPU powerUp, int level)
        {
            try
            {
                return powerUp switch
                {
                    // Stats 
                    UDPU.AttackDamageUp => UD.AttackIncreaseText[level],
                    UDPU.PhysicalDefenseUp => UD.PhysicalDefenseIncreaseText[level],
                    UDPU.MagicalDefenseUp => UD.MagicalDefenseIncreaseText[level],
                    UDPU.HpUp => UD.HpIncreaseText[level],
                    UDPU.CriticalRateUp => UD.CriticalChanceIncreaseText[level],
                    UDPU.CriticalDamageUp => UD.CriticalDamageIncreaseText[level],
                    
                    // Effects
                    UDPU.ElectricAttack => UD.ElectricAttackTexts[level],
                    UDPU.FireAttack => UD.FireAttackTexts[level],
                    UDPU.IceAttack => UD.IceAttackTexts[level],
                    UDPU.LifeStealUp => UD.LifeStealTexts[level],
                    
                    // Mechanics
                    UDPU.PerfectDodgeAttack => UD.PerfectTimingDodgeText[level],
                    UDPU.HitProjectiles => UD.HitProjectilesTexts[level],
                    
                    _ => "Error: Attack text not found."
                };
            }
            catch (IndexOutOfRangeException)
            {
                return "Error: Out of bounds on " + powerUp + " power up!";
            }
        }

        string GetPowerUpTitle(UDPU powerUp, int level)
        {
            try
            {
                return powerUp switch
                {
                    // Stats 
                    UDPU.AttackDamageUp => UD.AttackIncreaseTitle[level],
                    UDPU.PhysicalDefenseUp => UD.PhysicalDefenseIncreaseTitle[level],
                    UDPU.MagicalDefenseUp => UD.MagicalDefenseIncreaseTitle[level],
                    UDPU.HpUp => UD.HpIncreaseTitle[level],
                    UDPU.CriticalRateUp => UD.CriticalChanceIncreaseTitle[level],
                    UDPU.CriticalDamageUp => UD.CriticalDamageIncreaseTitle[level],

                    // Effects
                    UDPU.ElectricAttack => UD.ElectricAttackTitles[level],
                    UDPU.FireAttack => UD.FireAttackTitles[level],
                    UDPU.IceAttack => UD.IceAttackTitles[level],
                    UDPU.LifeStealUp => UD.LifeStealTitles[level],

                    // Mechanics
                    UDPU.PerfectDodgeAttack => UD.PerfectTimingDodgeTitle[level],
                    UDPU.HitProjectiles => UD.HitProjectilesTitles[level],

                    _ => "Error: Attack title not found."
                };
            }
            catch (IndexOutOfRangeException)
            {
                return "Error: Out of bounds on " + powerUp + " power up!";
            }
        }

    }
    
}
