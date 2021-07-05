using UnityEngine;

namespace Resources.Scripts.Enemy
{
    public class EnemyStatsManager : MonoBehaviour
    {
        public float MaxHP { get; private set; }
        public float PhysicalDamage { get; private set; }
        public float MagicalDamage { get; private set; }
        public float PhysicalDefense { get; private set; }
        public float MagicalDefense { get; private set; }
        public float MoveSpeed { get; private set; }
        public float AttackSpeed { get; private set; }
        public float AttackCooldown { get; private set; }
        public float ExpDropQuantity { get; private set; }
    }
}
