using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;

public class PlayerStatsController : MonoBehaviour
{
    private PowerUpController PowerUpController;
    
    /* Stats */
    [SerializeField] 
    private float BasePhysicalDamage = 40f;

    public float PhysicalDamage; // { get; private set; }

    [SerializeField] 
    private float BaseAttackSpeed = 40f;
    public float AttackSpeed; // Divides the attack cooldown
    
    [SerializeField] 
    private float BasePhysicalDefense = 20f;
    public float PhysicalDefense; // { get; private set; }
    [SerializeField] 
    private float BaseMagicalDefense = 20f;
    public float MagicalDefense; // { get; private set; } [SerializeField]
    [SerializeField]
    private float BaseMaxHp = 100f;
    public float MaxHp; // { get; private set; }
    [SerializeField] 
    private float BaseEvasionRate = 5f;
    public float EvasionRate; // { get; private set; }
    [SerializeField] private float BaseCriticalRate = 10f; 
    public float CriticalRate; // { get; private set; }
    [SerializeField]
    private float BaseCriticalMultiplier = 1.5f;
    public float CriticalDamage; // { get; private set; }
    
    /* Effects */
    
    // Fire
    [SerializeField] 
    private float BaseFireDamage = 25; 
    public float FireDamage; // { get; private set; }
    [SerializeField] 
    private float BaseFireAttackRate = 25f;
    public float FireAttackRate { get; private set; }
    
    // Ice
    [SerializeField] 
    private float BaseIceDamage = 20f;
    public float IceDamage; // { get; private set; }
    [SerializeField] 
    private float BaseIceAttackRate = 15f;
    public float IceAttackRate; // { get; private set; }
    [SerializeField] 
    private float BaseFrostDuration = 1f;
    public float FrostDuration; // { get; private set; }
    [SerializeField] 
    private float BaseShatterDamage = 50f;
    public float ShatterDamage; // { get; private set; }

    // Electrical
    [SerializeField] private float BaseElectricalDamage = 30f;
    public float ElectricalDamage; // { get; private set; }
    [SerializeField] 
    private float BaseElectricAttackRate = 30f;
    public float ElectricAttackRate; // { get; private set; }
    [SerializeField] 
    private float BaseElectricRange = 8f;
    public float ElectricRange; // { get; private set; }
    private float BaseParalyzeDuration = 1.5f;
    public float ParalyzeDuration; // { get; private set; } 
    
    // Life steal
    private float BaseLifeStealPercentage = 5f;
    public float LifeStealPercentage; // { get; private set; }
    private float BaseLifeStealAttackRate = 20f;
    public float LifeStealAttackRate; // { get; private set; }
    
    private void Awake()
    {
        PowerUpController = GetComponent<PowerUpController>();
        UpdateAll();
    }

    private void Update()
    {
        // DEBUG
        if (Input.GetKeyDown(KeyCode.U)) UpdateAll();
    }
    
    // Stats 
    public void UpdatePhysicalDamage()
    {
        PhysicalDamage = (float) (BasePhysicalDamage * Math.Log(PowerUpController.AttackLevel + 2, 4.0)); // +2 for Log reasons
    }
    
    public void UpdateAttackSpeed()
    {
        AttackSpeed = (float) (BaseAttackSpeed * Math.Log(PowerUpController.AttackSpeedLevel + 2, 4.0)); // +2 for Log reasons
    }   
    public void UpdatePhysicalDefense()
    {
        PhysicalDefense = (float) (BasePhysicalDefense * Math.Log(PowerUpController.PhysicalDefenseLevel + 2, 4.0)); // +2 for Log reasons
    }
    public void UpdateMagicalDefense()
    {
        MagicalDefense = (float) (BaseMagicalDefense * Math.Log(PowerUpController.MagicalDefenseLevel + 2, 4.0)); // +2 for Log reasons
        
    }
    public void UpdateMaxHp()
    {
        MaxHp = BaseMaxHp + (BaseMaxHp * PowerUpController.HpLevel) / 3; 
    }
    public void UpdateEvasionRate()
    {
        EvasionRate =  (float) (BaseEvasionRate * Math.Log(PowerUpController.EvasionLevel + 2, 4)); // +2 for Log reasons
    }
    public void UpdateCriticalRate()
    {
        CriticalRate = (float) (BaseCriticalRate * Math.Log(PowerUpController.CriticalRateLevel + 2, 2)); // +2 for Log reasons
    }
    public void UpdateCriticalDamage()
    {
        CriticalDamage = (float) (BaseCriticalMultiplier * Math.Log(PowerUpController.CriticalRateLevel + 2, 2)); // +2 for Log reasons
    }
    
    /* Effects */
    // Fire
    public void UpdateFireDamage()
    {
        FireDamage = (float) (BaseFireDamage * Math.Log(PowerUpController.FireLevel + 2, 4.0)); // +2 for Log reasons
    }
    public void UpdateFireAttackRate()
    {
        FireAttackRate = (float) (BaseFireAttackRate * Math.Log(PowerUpController.FireLevel + 2, 4.0)); // +2 for Log reasons
    }
    
    // Ice
    public void UpdateIceDamage()
    {
        IceDamage = (float) (BaseIceDamage * Math.Log(PowerUpController.IceLevel + 2, 4.0)); // +2 for Log reasons
    }
    public void UpdateIceAttackRate()
    {
        IceAttackRate = (float) (BaseIceAttackRate * Math.Log(PowerUpController.IceLevel + 2, 4.0)); // +2 for Log reasons
    }

    public void UpdateFrostDuration()
    {
        FrostDuration = (float) (BaseFrostDuration * Math.Log(PowerUpController.IceLevel + 2, 4.0));
    }
    public void UpdateShatterDamage()
    {
        ShatterDamage = (float) (BaseShatterDamage * Math.Log(PowerUpController.IceLevel + 2, 4.0));
    }
    
    // Electrical
    public void UpdateElectricalDamage()
    {
        ElectricalDamage = (float) (BaseElectricalDamage * Math.Log(PowerUpController.ElectricLevel + 2, 4.0)); // +2 for Log reasons
    }
    public void UpdateElectricAttackRate()
    {
        ElectricAttackRate = (float) (BaseElectricAttackRate * Math.Log(PowerUpController.ElectricLevel + 2, 4.0)); // +2 for Log reasons
    }
    public void UpdateElectricRange()
    {
        ElectricRange = (float) (BaseElectricRange * Math.Log(PowerUpController.ElectricLevel + 2, 4.0));
    }

    public void UpdateParalyzeDuration()
    {
        ParalyzeDuration = (float) (BaseParalyzeDuration * Math.Log(PowerUpController.ElectricLevel + 2, 4.0));
    }

    // Life Steal
    public void UpdateLifeStealPercentage()
    {
        LifeStealPercentage = (float) (BaseLifeStealPercentage * Math.Log(PowerUpController.LifeStealLevel + 2, 4.0)); // +2 for Log reasons
    }
    public void UpdateLifeStealAttackRate()
    {
        LifeStealAttackRate = (float) (BaseLifeStealAttackRate * Math.Log(PowerUpController.LifeStealLevel + 2, 4.0)); // +2 for Log reasons
    }

    public void UpdateAll()
    {
        UpdatePhysicalDamage();
        UpdateAttackSpeed();
        UpdatePhysicalDefense();
        UpdateMagicalDefense();
        UpdateMaxHp();
        UpdateEvasionRate();
        UpdateCriticalRate();
        UpdateCriticalDamage();
        UpdateFireDamage();
        UpdateIceDamage();
        UpdateFrostDuration();
        UpdateShatterDamage();
        UpdateElectricalDamage();
        UpdateElectricRange();
        UpdateParalyzeDuration();
        UpdateLifeStealPercentage();
        UpdateFireAttackRate();
        UpdateIceAttackRate();
        UpdateElectricAttackRate();
        UpdateLifeStealAttackRate();
    }
}
