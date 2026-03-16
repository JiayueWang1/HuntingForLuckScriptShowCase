using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

namespace MetroidvaniaTools
{
    public class Stagger : Node
    {
        // Pass In
        private NormalEnemyCharacter enemyCharacter;
        private EnemyHealth enemyHealth;
        private float knockBackDistance;
        private float knockBackHeight;
        private float staggerTime;

        // Local
        private bool canTakeNextHit;
        private float staggerCountDown;

        public Stagger(NormalEnemyCharacter enemyCharacter, EnemyHealth enemyHealth, float knockBackDistance,
            float knockBackHeight, float staggerTime)
            => (this.enemyCharacter, this.enemyHealth, this.knockBackDistance, this.knockBackHeight, this.staggerTime) =
                (enemyCharacter, enemyHealth, knockBackDistance, knockBackHeight, staggerTime);
        
        public override NodeState Evaluate()
        {
            if (!enemyHealth.hit && enemyCharacter.curState != NormalEnemyStates.STAGGER)
            {
                state = NodeState.FAILURE;
                return state;
            }

            if (enemyCharacter.curState != NormalEnemyStates.STAGGER)
            {
                // First Enter
                enemyCharacter.curState = NormalEnemyStates.STAGGER;
                TriggerKnockBack();
            }

            if (!enemyHealth.hit)
                canTakeNextHit = true;
            
            if (canTakeNextHit && enemyHealth.hit)
            {
                TriggerKnockBack();
            }

            if (staggerCountDown <= 0)
            {
                enemyCharacter.curState = NormalEnemyStates.NULL;
                enemyCharacter.anim.SetTrigger("StaggerEnd");
                state = NodeState.SUCCESS;
            }
            else
            {
                state = NodeState.RUNNING;
                staggerCountDown -= Time.deltaTime;
            }
            enemyCharacter.EdgeProtection();
            return state;
        }

        private void TriggerKnockBack()
        {
            canTakeNextHit = false;
            staggerCountDown = staggerTime;
            enemyCharacter.rb.velocity = new Vector2(0, 0);
            enemyCharacter.anim.SetTrigger("Stagger");
            float knockBackDir = -1;
            if (enemyCharacter.player.transform.position.x < enemyCharacter.transform.position.x)
            {
                knockBackDir = 1;
            }
            enemyCharacter.rb.AddForce(new Vector2(knockBackDir * knockBackDistance * enemyCharacter.rb.gravityScale
                , knockBackHeight), ForceMode2D.Impulse);
        }
    }   
}
