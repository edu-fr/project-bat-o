using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLevelController : MonoBehaviour
{
    private int CurrentLevel;

    [SerializeField] private int MaxLevel;
    [SerializeField] private float ExperienceMultiplier; 
    private int ExpToNextLevel;
    private int CurrentLevelExp;
    private int TotalExp;
   
    
    private void Awake()
    {
        CurrentLevel = 1;
        ExpToNextLevel = 10;
    }

    private void Update()
    {
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
        ExpToNextLevel += (int) (ExpToNextLevel * 0.10f);
        // Call the power up screen
    }

    public void GetExperience(int amount)
    {
        CurrentLevelExp += amount;
        TotalExp += amount;
        // Play Get Exp animation
    }
}