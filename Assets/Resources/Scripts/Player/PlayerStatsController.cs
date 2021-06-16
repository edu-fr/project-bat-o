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
    private float BasePhysicalDamage;
    public float PhysicalDamage { get; private set; }
    [SerializeField] 
    private float BasePhysicalDefense;
    public float PhysicalDefense { get; private set; }
    [SerializeField] 
    private float BaseMagicalDefense;
    public float MagicalDefense { get; private set; } [SerializeField]
    private float BaseMaxHp;
    public float MaxHp { get; private set; }
    [SerializeField] 
    private float BaseEvasionRate;
    public float EvasionRate { get; private set; }
    [SerializeField] 
    private float BaseCriticalRate;
    public float CriticalRate { get; private set; }
    [SerializeField]
    private float BaseCriticalDamage;
    public float CriticalDamage { get; private set; }
    
    /* Effects */
    
    // Fire
    [SerializeField] 
    private float BaseFireDamage = 25; 
    public float FireDamage { get; private set; }
    [SerializeField] 
    private float BaseFireAttackRate = 25f;
    public float FireAttackRate { get; private set; }
    
    // Ice
    [SerializeField] 
    private float BaseIceDamage;
    public float IceDamage { get; private set; }
    [SerializeField] 
    private float BaseIceAttackRate;
    public float IceAttackRate { get; private set; }
    [SerializeField] 
    private float BaseFrostDuration = 1f;
    public float FrostDuration { get; private set; }
    [SerializeField] 
    private float BaseShatterDamage = 50f;
    public float ShatterDamage { get; private set; }

    // Electrical
    [SerializeField] private float BaseElectricalDamage;
    public float ElectricalDamage { get; private set; }
    [SerializeField] 
    private float BaseElectricAttackRate;
    public float ElectricAttackRate { get; private set; }
    [SerializeField] 
    private float BaseElectricRange;
    public float ElectricRange { get; private set; }
    private float BaseParalyzeDuration;
    public float ParalyzeDuration; 
    
    // Life steal
    public float BaseLifeStealPercentage { get; private set; }
    public float LifeStealPercentage { get; private set; }
    public float BaseLifeStealAttackRate { get; private set; }
    public float LifeStealAttackRate { get; private set; }
    
    private void Awake()
    {
        PowerUpController = GetComponent<PowerUpController>();
    }
    
    public void UpdatePhysicalDamage()
    {
        PhysicalDamage = (float) (BasePhysicalDamage * Math.Log(PowerUpController.AttackLevel + 2, 4.0)); // +2 for Log reasons
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
        CriticalRate = (float) (BaseCriticalRate * Math.Log(PowerUpController.CriticalRateLevel + 2, 4)); // +2 for Log reasons
    }
    
    public void UpdateCriticalDamage()
    {
        CriticalDamage = (float) (BaseCriticalDamage * Math.Log(PowerUpController.CriticalRateLevel + 2, 4)); // +2 for Log reasons
    }
    
    public void UpdateFireDamage()
    {
        FireDamage = (float) (BaseFireDamage * Math.Log(PowerUpController.FireLevel + 2, 4.0)); // +2 for Log reasons
    }
    
    public void UpdateIceDamage()
    {
        IceDamage = (float) (BaseIceDamage * Math.Log(PowerUpController.IceLevel + 2, 4.0)); // +2 for Log reasons
    }

    public void UpdateElectricalDamage()
    {
        ElectricalDamage = (float) (BaseElectricalDamage * Math.Log(PowerUpController.ElectricLevel + 2, 4.0)); // +2 for Log reasons
    }

    public void UpdateLifeStealPercentage()
    {
        LifeStealPercentage = (float) (BaseLifeStealPercentage * Math.Log(PowerUpController.LifeStealLevel + 2, 4.0)); // +2 for Log reasons
    }

    public void UpdateFireAttackRate()
    {
        FireAttackRate = (float) (BaseFireAttackRate * Math.Log(PowerUpController.FireLevel + 2, 4.0)); // +2 for Log reasons
    }

    public void UpdateIceAttackRate()
    {
        IceAttackRate = (float) (BaseIceAttackRate * Math.Log(PowerUpController.IceLevel + 2, 4.0)); // +2 for Log reasons
    }

    public void UpdateElectricAttackRate()
    {
        ElectricAttackRate = (float) (BaseElectricAttackRate * Math.Log(PowerUpController.ElectricLevel + 2, 4.0)); // +2 for Log reasons
    }

    public void UpdateLifeStealAttackRate()
    {
        LifeStealAttackRate = (float) (BaseLifeStealAttackRate * Math.Log(PowerUpController.LifeStealLevel + 2, 4.0)); // +2 for Log reasons
    }
}
