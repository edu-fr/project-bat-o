using System.Collections;
using Player;
using UnityEngine;

namespace Enemy.Attacks
{
    public class DashAttack : BaseAttack
    {
        public BoxCollider2D AttackHitbox;
        public override void PreparingAttack()
        {
            // make sound once (?)
            EnemyCombatManager.Rigidbody2D.AddForce(-EnemyStateMachine.PlayerDirection * PreparationWalkDistance, ForceMode2D.Force);
        }
        
        public override void Attack(Vector3 playerDirection)
        {
            AttackHitbox.enabled = true;
            if(hasAttackAnimation)
                EnemyAnimationController.AnimateAttack(playerDirection.x, playerDirection.y);
            EnemyCombatManager.Rigidbody2D.AddForce(playerDirection * (EnemyStatsManager.CurrentAttackSpeed * attackSpeedModifier), ForceMode2D.Impulse);
            EnemyCombatManager.IsAttacking = true;
            if(!hasAttackAnimation)
                AttackEnd();
        }
        
        public override void AttackEnd()
        {
            base.AttackEnd();
            isOnHalfOfAttackAnimation = false;
            StartCoroutine(DeactivateAttackHitBox(0.4f));
        }
        
        public void SetIsOnHalfOfAttackAnimation() // called by animator (?)
        {
            isOnHalfOfAttackAnimation = true;
        }

        private IEnumerator DeactivateAttackHitBox(float timeToDeactivate)
        {
            yield return new WaitForSeconds(timeToDeactivate);
            AttackHitbox.enabled = false;
        }
    }
}