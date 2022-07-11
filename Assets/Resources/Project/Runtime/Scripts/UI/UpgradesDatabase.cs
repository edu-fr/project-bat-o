namespace Resources.Project.Runtime.Scripts.UI
{ 
    public static class UpgradesDatabase
    {
        public enum PowerUps : int
        {
            // Stats
            AttackDamageUp = 1,
            PhysicalDefenseUp = 2,
            MagicalDefenseUp = 3,
            HpUp = 4,
            CriticalRateUp = 5,
            CriticalDamageUp = 6,
        
            // Effects
            FireAttack = 10,
            IceAttack = 11,
            ElectricAttack = 12,
            LifeStealUp = 13, 
            
            // Mechanical 
            PerfectDodgeAttack = 100,
            HitProjectiles = 101,
        }

        public static int StatsBottomIndex = 1;
        public static int StatsTopIndex = 6;
        
        public static int EffectsBottomIndex = 10;
        public static int EffectsTopIndex = 13;

        public static int MechanicalBottomIndex = 100;
        public static int MechanicalTopIndex = 101;

        // STATS POWER UPS

        public static readonly string[] AttackIncreaseTitle =
        {
            "Attack damage up"
        };
        
        public static readonly string[] AttackIncreaseText =
        {
            "Your basic attacks deal more damage."
        };
        
        public static readonly string[] PhysicalDefenseIncreaseTitle =
        {
            "Physical defense up"
        };
        
        public static readonly string[] PhysicalDefenseIncreaseText =
        {
            "You take less damage from enemies physical attacks."
        };
        
        public static readonly string[] MagicalDefenseIncreaseTitle =
        {
            "Magical defense up"
        };
        
        public static readonly string[] MagicalDefenseIncreaseText =
        {
            "You take less elemental and magic damage."
        };

        
        public static readonly string[] HpIncreaseTitle =
        {
            "Maximum health up"
        };

        
        public static readonly string[] HpIncreaseText =
        {
            "Increase your maximum health points."
        };
        
        public static readonly string[] CriticalChanceIncreaseTitle =
        {
            "Critical hit chance up"
        };
        
        public static readonly string[] CriticalChanceIncreaseText =
        {
            "Increases your critical hit rate."
        };
        
        public static readonly string[] CriticalDamageIncreaseTitle =
        {
            "Critical hit damage up"
        };
        
        public static readonly string[] CriticalDamageIncreaseText =
        {
            "Increases your critical hit damage."
        };
        
        // EFFECT POWER UPS
        
        public static readonly string[] FireAttackTitles =
        {
            "Heat slash",
            "Fire slash"
        };
        
        public static readonly string[] FireAttackTexts =
        {
            "Your attacks may set the enemy on fire for a short period of time.",
            "While burning, enemies take more damage from all sources and will run from you if they were to be killed by the fire."
        };

        public static readonly string[] IceAttackTitles =
        {
            "Cold slash",
            "Ice slash"
        };
        
        public static readonly string[] IceAttackTexts =
        {
            "Your attacks may freeze the enemy for a short period of time.",
            "The freeze caused by your attacks will last longer. Attacking a frozen enemy will unfreeze it and do critical damage to it."
        };

        public static readonly string[] ElectricAttackTitles =
        {
            "Energized slash",
            "Electrical slash"
        };
        
        public static readonly string[] ElectricAttackTexts =
        {
            "Your attacks can deal electrical damage to the enemy and two other nearby enemies.",
            "Your electrical attacks may paralyze the enemy for a average period of time."
        };
        
        public static readonly string[] LifeStealTitles =
        {
            "Life steal"
        };
        
        public static readonly string[] LifeStealTexts =
        {
            "Has a chance of recover a small amount of HP on hit.",
            "Increase life steal hits "
        };
        
        // MECHANICAL POWER UPS
        
        public static readonly string[] PerfectTimingDodgeTitle =
        {
            "Perfect dodge"
        };
        
        public static readonly string[] PerfectTimingDodgeText =
        {
            "Dodge just before being hit by melee attacks slows down the flow of time and gives you the opportunity to land a free critical hit."
        };
        
        public static readonly string[] HitProjectilesTitles =
        {
            "Hit projectiles ability",
            "Elemental projectiles hit"
        };
        
        public static readonly string[] HitProjectilesTexts =
        {
            "Gives you the ability to bounce enemy projectiles back to them with your sword.",
            "Your sword elemental effects can be applied to projectiles you hit",
        };

    }
}
