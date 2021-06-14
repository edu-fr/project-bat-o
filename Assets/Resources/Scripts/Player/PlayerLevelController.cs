using System.Collections;
using System.Collections.Generic;
using Resources.Scripts.UI;
using UnityEngine;

public class PlayerLevelController : MonoBehaviour
{
    private int CurrentLevel;

    [SerializeField] 
    private int MaxLevel = 10;
    [SerializeField] 
    private float ExperienceMultiplier = 0.1f;
    private int ExpToNextLevel;
    private int CurrentLevelExp;
    private int TotalExp;

    [SerializeField] 
    private GameObject Canvas;

    private LevelUpMenu LevelUpMenu;
    
    private void Awake()
    {
        LevelUpMenu = Canvas.GetComponent<LevelUpMenu>();
        
        CurrentLevel = 1;
        CurrentLevelExp = 0;
        TotalExp = 0;
        ExpToNextLevel = 10;
    }

    private void Update()
    {
        // Debug

        if(Input.GetKeyDown(KeyCode.L))
        {
            LevelUp();
        }

        if (CurrentLevel >= MaxLevel) return;
        
        if (CurrentLevelExp > ExpToNextLevel)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        // Play level up animation
        CurrentLevel++;
        CurrentLevelExp = 0;
        ExpToNextLevel += (int) (ExpToNextLevel * ExperienceMultiplier);
        this.LevelUpMenu.OpenLevelUpMenu();
    }
    
    public void GainExperience(int amount)
    {
        CurrentLevelExp += amount;
        TotalExp += amount;
        // Play Get Exp animation
    }
}