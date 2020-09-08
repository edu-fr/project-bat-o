using System;
using System.Collections.Generic;
using Player;
using UnityEditor.Rendering;
using UnityEngine;

namespace Enemy
{
    public class ArrowParticleScript : MonoBehaviour
    {
        public ParticleSystem ParticleSystem;
        public EnemyCombatManager EnemyCombatManager;


        private List<ParticleCollisionEvent> CollisionEvents = new List<ParticleCollisionEvent>();

        private void Awake()
        {
            EnemyCombatManager = GetComponentInParent<EnemyCombatManager>();
        }
        
        private void Update()
        {
            
        }
        
        private void OnParticleCollision(GameObject other)
        {
            int events = ParticleSystem.GetCollisionEvents(other, CollisionEvents);

            if (other.TryGetComponent(out PlayerHealthManager playerHealthManager))
            {
                playerHealthManager.TakeDamage((int) EnemyCombatManager.RangedDamage);
            }
        }

        public void ShootArrow(Vector3 playerDirection)
        {
            ParticleSystem.transform.forward = playerDirection;
            ParticleSystem.MainModule particleSystemMain = ParticleSystem.main;
            particleSystemMain.startRotation = ((playerDirection.x * playerDirection.x) + (playerDirection.y + playerDirection.y))/2;
            ParticleSystem.Play();
        }
    }
}
