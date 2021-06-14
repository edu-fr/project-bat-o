using Enemy;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

namespace Player
{
    public class PowerUpController : MonoBehaviour
    {
        public enum Effects
        {
            Fire,
            Ice,
            Thunder,
            None
        };

        public int FireLevel = 0;
        public int IceLevel = 0;
        public int ElectricLevel = 0;
        public int HpLevel = 0;
        public int AttackLevel = 0;
        public int PhysicalDefenseLevel = 0;
        public int MagicalDefenseLevel = 0;
        public int CriticalRateLevel = 0;
        public int CriticalDamageLevel = 0;
        public int LifeStealLevel = 0;
        public int PerfectDodgeLevel = 0;
        public int DeflectArrowsLevel = 0;

        // Values

        public int OddsFireLv1 = 25; // 25% chance of burning the enemy 
        public int FireDamageLv1 = 25; // Deals 25 damage in 4 ticks
        public int FireDamageLv2 = 40; // Deals 40 damage in 4 ticks

        public float DefrostTimeLv1 = 1f; // Freeze the enemy for 1 second
        public float DefrostTimeLv2 = 1.5f; // Freeze the enemy for 2 seconds
        public int OddsIceLv1 = 15; // 15% chance of freezing the enemy
        public int ShatterDamage = 50; // Deals 50 damage to a frozen enemy 

        public int OddsThunderLv1 = 20; // 20% chance of deal electric damage to an enemy and up to two enemies nearby

        public float ElectricRange = 1.7f; // Up to two enemies on this range will receive electric damage
        public int ElectricDamageLv1 = 10; // Deals 10 damage to an enemy and up to two enemies nearby
        public int ElectricDamageLv2 = 15; // Deals 15 damage to an enemy and up to two enemies nearby

        public float ParalyzeTime = 1.5f; // Paralyze the enemy  for 1.5 seconds
        public int OddsThunderLv2 = 50; // 50% chance of paralyze the primary target hit by and electric attack
        
        public float DamageUpMultiplier = 0.15f; // Increase the player attack by 15%
        public float HpUpMultiplier = 0.10f; // Increase the player max health by 10%
        public float HealPercentage = 0.4f; // Recover 40% of the player's max hp
        
        private bool FireActivate;
        private bool IceActivate;
        private bool ElectricActivate;
    }
}