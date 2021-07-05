using System;
using System.Collections;
using System.Collections.Generic;
using Enemy;
using Player;
using Resources.Scripts.Enemy;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    [SerializeField]
    private float Damage;

    private Vector3 ShootDirection;
    private float MoveSpeed;

    private bool IsStuck;
    public void Setup(Vector3 shootDirection, float projectileSpeed)
     {
         this.ShootDirection = shootDirection;
         this.MoveSpeed = projectileSpeed;
         
         transform.right = shootDirection;
         Destroy(gameObject, 3f);
     }

     private void Update()
     {
         if(!IsStuck) transform.position += ShootDirection * (MoveSpeed * Time.deltaTime);
     }

     private void OnTriggerEnter2D(Collider2D other)
     {
         if (other.gameObject.CompareTag("Player"))
         {
             // Hurt player
             other.gameObject.GetComponent<PlayerHealthManager>()?.TakeDamage(Damage);
             Destroy(gameObject);
         }

         if (other.gameObject.CompareTag("Enemy"))
         {
             // Hurt enemy
             other.gameObject.GetComponent<EnemyCombatManager>()?.TakeDamage(Damage, ShootDirection, 25, false, false, true, Color.yellow); // arbitrary attack speed
             Destroy(gameObject);
         }

         if (other.gameObject.CompareTag("Scene objects"))
         {
             IsStuck = true;
         }
     }

     public void ReflectProjectile()
     {
         ShootDirection = -ShootDirection;
         MoveSpeed = MoveSpeed * 1.5f;
         transform.right = ShootDirection;
     }
}
