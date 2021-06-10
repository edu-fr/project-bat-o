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
            ElectricAttack,

            // Mechanical abilities
            PerfectDodgeAttack = 8
        }

        public static int StatsBottomIndex = 1;
        public static int StatsTopIndex = 4;
        
        public static int EffectsBottomIndex = 5;
        public static int EffectsTopIndex = 7;

        public static int MechanicalBottomIndex = 8;
        public static int MechanicalTopIndex = 8;

        public static string[] FireAttackTitles =
        {
            "Heat slash",
            "Fire slash"
        };
        
        public static string[] FireAttackTexts =
        {
            "Your attacks may set the enemy on fire for a short period of time.",
            "While burning, enemies take more damage from all sources and will run from you if they were to be killed by the fire."
        };

        public static string[] IceAttackTitles =
        {
            "Cold slash",
            "Ice slash"
        };
        
        public static string[] IceAttackTexts =
        {
            "Your attacks may freeze the enemy for a short period of time.",
            "The freeze caused by your attacks will last longer. Attacking a frozen enemy will unfreeze it and do critical damage to it."
        };

        public static string[] ElectricAttackTitles =
        {
            "Energized slash",
            "Electrical slash"
        };
        
        public static string[] ElectricAttackTexts =
        {
            "Your attacks can deal electrical damage to the enemy and two other nearby enemies.",
            "Your electrical attacks may paralyze the enemy for a average period of time."
        };
    }
}
