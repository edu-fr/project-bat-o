using System;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

namespace Enemy
{
    public class ArrowParticleScript : MonoBehaviour
    {
        public ParticleSystem ParticleSystem;

        private List<ParticleCollisionEvent> CollisionEvents = new List<ParticleCollisionEvent>();

        private void Update()
        {
            
        }
        
        private void OnParticleCollision(GameObject other)
        {
            int events = ParticleSystem.GetCollisionEvents(other, CollisionEvents);

            Debug.Log("COLIDIU");
            for (int i = 0; i < events; i++)
            {
                
            }
        }

        public void ShootArrow()
        {
            ParticleSystem.Play();
        }
    }
}
