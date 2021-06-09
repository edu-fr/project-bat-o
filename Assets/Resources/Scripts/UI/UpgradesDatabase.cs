using UnityEngine;

namespace Resources.Scripts.UI
{ 
    public static class UpgradesDatabase
    {
        public enum PowerUps : int
        {
            // Stats
            AttackDamageUp = 1,
            DefenseUp = 2,
            ResistanceUp = 3,
            HpUp = 4,
        
            // Effects
            FireAttack = 5,
            IceAttack = 6,
            ElectricAttack = 7,

            // Mechanical abilities
            PerfectDodgeAttack = 8
        }

        public static int StatsBottomIndex = 1;
        public static int StatsTopIndex = 4;
        
        public static int EffectsBottomIndex = 5;
        public static int EffectsTopIndex = 7;

        public static int MechanicalBottomIndex = 8;
        public static int MechanicalTopIndex = 8;
        
    }
}
