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

        // Stats
        public int HpLevel = 0;
        public int AttackLevel = 0;
        public int PhysicalDefenseLevel = 0;
        public int MagicalDefenseLevel = 0;
        public int CriticalRateLevel = 0;
        public int CriticalDamageLevel = 0;
        public int EvasionLevel = 0;
        
        // Effects
        public int FireLevel = 0;
        public int IceLevel = 0;
        public int ElectricLevel = 0;
        public int LifeStealLevel = 0;
        
        // Mechanical
        public int PerfectDodgeLevel = 0;
        public int HitProjectilesLevel = 0;

        // Values
        private bool FireActivate;
        private bool IceActivate;
        private bool ElectricActivate;
    }
}