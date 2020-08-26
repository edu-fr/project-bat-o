using UnityEngine;

namespace Enemy
{
    public class EnemyRangedAttackManager : MonoBehaviour
    { 
        public ParticleSystem ParticleSystem;
        private ArrowParticleScript ArrowParticleScript;

        private void Awake()
        {
            ArrowParticleScript = ParticleSystem.GetComponent<ArrowParticleScript>();
        }
        
        public void Attack()
        {
            ArrowParticleScript.ShootArrow();
        }
    }
}
