using System;
using System.Collections;
using System.Collections.Generic;
using Enemy;
using Player;
using UI;
using UnityEngine;

public class ObjectDamageScript : MonoBehaviour
{
    public Collider2D ObjectCollider2D;
    public Transform PrefabDamagePopup;
    [SerializeField]
    private float ObjectDamage;
    [SerializeField] 
    private float ObjectKnockBack;
    [SerializeField] 
    private float ObjectKnockBackDuration;
    [SerializeField] 
    private float ObjectAttackSpeed; // the bigger, the less time the attacker is invulnerable
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            var damageTakenByEnemy = other.gameObject.GetComponent<EnemyCombatManager>().TakeDamage(ObjectDamage, ObjectKnockBack, 
                other.transform.position - transform.position, ObjectKnockBackDuration, 1);
            DamagePopup.Create(other.transform.position, (int) damageTakenByEnemy, false, other.transform.position - transform.position,
                PrefabDamagePopup);
        }
        else if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerHealthManager>().TakeDamage(ObjectDamage);
        }
    }
}
