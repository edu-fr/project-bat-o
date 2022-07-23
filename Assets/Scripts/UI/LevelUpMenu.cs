using System;
using System.Collections.Generic;
using Player;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace UI
{
    public class LevelUpMenu : MonoBehaviour
    {
    public GameObject LevelUpUI;

    private List<TextMeshProUGUI> Titles;
    private List<TextMeshProUGUI> Texts;
    
    public static bool IsLevelingUp;
    // private List<UDPU> Options;
    
    private PowerUpController PlayerPowerUpController;
    private PlayerStatsController PlayerStatsController;
    
    public void OpenLevelUpMenu()
    {
    //     Options = GetOptions();
        for (int i = 0; i < Titles.Count; i++)
        {
       //     ConfigureButton(Titles[i], Texts[i], Options[i]);
        } ;
        LevelUpUI.SetActive(true);
        Time.timeScale = 0f;
        IsLevelingUp = true;
    }
    
    public void FirstPowerUpClicked()
    {
        // ApplyPowerUp(Options[0]);   
        CloseLevelUpMenu();
    }
    
    public void SecondPowerUpClicked()
    {
        // ApplyPowerUp(Options[1]);   
        CloseLevelUpMenu();
    }
    
    public void ThirdPowerUpClicked()
    {
        // ApplyPowerUp(Options[2]);  
        CloseLevelUpMenu();
    }
    
    private void CloseLevelUpMenu()
    {
        LevelUpUI.SetActive(false);
        Time.timeScale = 1f;
        IsLevelingUp = false;
    }
    //
    // private List<UDPU> GetOptions()
    // {
    //     var options = new List<UDPU>();
    //     // First option (Effect)
    //     Random.InitState((int) DateTime.Now.Ticks);
    //     var firstOption = Random.Range(UD.EffectsBottomIndex, UD.EffectsTopIndex + 1);
    //     options.Add((UDPU) firstOption);
    //     
    // }
    //
    // void ConfigureButton(TextMeshProUGUI title, TextMeshProUGUI text, UDPU selectedPowerUp)
    // {
    //     var powerUpInfo = GetPowerUpInfo(selectedPowerUp);
    //     title.SetText(GetPowerUpTitle(selectedPowerUp, powerUpInfo));
    //     text.SetText(GetPowerUpText(selectedPowerUp, powerUpInfo));
    // }
    //
    // int GetPowerUpInfo(UDPU powerUp)
    // {
    //     return powerUp switch
    //     {
    //         // Stats
    //         UDPU.AttackDamageUp => PlayerPowerUpController.AttackLevel,
    //         UDPU.PhysicalDefenseUp => PlayerPowerUpController.PhysicalDefenseLevel,
    //         UDPU.MagicalDefenseUp => PlayerPowerUpController.PhysicalDefenseLevel,
    //         UDPU.HpUp => PlayerPowerUpController.HpLevel,
    //         UDPU.CriticalRateUp => PlayerPowerUpController.CriticalRateLevel,
    //         UDPU.CriticalDamageUp => PlayerPowerUpController.CriticalDamageLevel,
    //         
    //         // Effects
    //         UDPU.FireAttack => PlayerPowerUpController.FireLevel,
    //         UDPU.ElectricAttack => PlayerPowerUpController.ElectricLevel,
    //         UDPU.IceAttack => PlayerPowerUpController.IceLevel,
    //         UDPU.LifeStealUp => PlayerPowerUpController.LifeStealLevel,
    //         
    //         // Mechanical
    //         UDPU.PerfectDodgeAttack => PlayerPowerUpController.PerfectDodgeLevel,
    //         UDPU.HitProjectiles => PlayerPowerUpController.HitProjectilesLevel,
    //         
    //         _ => 0
    //     };
    // }
    //
    // string GetPowerUpText(UDPU powerUp, int level)
    // {
    //     try
    //     {
    //         return powerUp switch
    //         {
    //             // Stats 
    //             UDPU.AttackDamageUp => UD.AttackIncreaseText[level],
    //             UDPU.PhysicalDefenseUp => UD.PhysicalDefenseIncreaseText[level],
    //             UDPU.MagicalDefenseUp => UD.MagicalDefenseIncreaseText[level],
    //             UDPU.HpUp => UD.HpIncreaseText[level],
    //             UDPU.CriticalRateUp => UD.CriticalChanceIncreaseText[level],
    //             UDPU.CriticalDamageUp => UD.CriticalDamageIncreaseText[level],
    //             
    //             // Effects
    //             UDPU.ElectricAttack => UD.ElectricAttackTexts[level],
    //             UDPU.FireAttack => UD.FireAttackTexts[level],
    //             UDPU.IceAttack => UD.IceAttackTexts[level],
    //             UDPU.LifeStealUp => UD.LifeStealTexts[level],
    //             
    //             // Mechanics
    //             UDPU.PerfectDodgeAttack => UD.PerfectTimingDodgeText[level],
    //             UDPU.HitProjectiles => UD.HitProjectilesTexts[level],
    //             
    //             _ => "Error: Attack text not found."
    //         };
    //     }
    //     catch (IndexOutOfRangeException)
    //     {
    //         return "Error: Out of bounds on " + powerUp + " power up!";
    //     }
    // }
    //
    // string GetPowerUpTitle(UDPU powerUp, int level)
    // {
    //     try
    //     {
    //         return powerUp switch
    //         {
    //             // Stats 
    //             UDPU.AttackDamageUp => UD.AttackIncreaseTitle[level],
    //             UDPU.PhysicalDefenseUp => UD.PhysicalDefenseIncreaseTitle[level],
    //             UDPU.MagicalDefenseUp => UD.MagicalDefenseIncreaseTitle[level],
    //             UDPU.HpUp => UD.HpIncreaseTitle[level],
    //             UDPU.CriticalRateUp => UD.CriticalChanceIncreaseTitle[level],
    //             UDPU.CriticalDamageUp => UD.CriticalDamageIncreaseTitle[level],
    //
    //             // Effects
    //             UDPU.ElectricAttack => UD.ElectricAttackTitles[level],
    //             UDPU.FireAttack => UD.FireAttackTitles[level],
    //             UDPU.IceAttack => UD.IceAttackTitles[level],
    //             UDPU.LifeStealUp => UD.LifeStealTitles[level],
    //
    //             // Mechanics
    //             UDPU.PerfectDodgeAttack => UD.PerfectTimingDodgeTitle[level],
    //             UDPU.HitProjectiles => UD.HitProjectilesTitles[level],
    //
    //             _ => "Error: Attack title not found."
    //         };
    //     }
    //     catch (IndexOutOfRangeException)
    //     {
    //         return "Error: Out of bounds on " + powerUp + " power up!";
    //     }
    // }
    
    }
}
