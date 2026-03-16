using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

namespace MetroidvaniaTools
{
    public class JumpAttack : Node
    {
        // Passing In
        private NormalEnemyCharacter enemyCharacter;
        private float detectRange;
        private float jumpHeight;
        private float preMoveTime;
        private float postMoveTime;
        private float distanceOffset;
        private float maxDistance;
        
        // Local
        // private Transform moveTarget;
        private float preMoveCountDown;
        private float postMoveCountDown;
        private int moveIndex; // 0 for premove, 1 for air, 2 for postmove

        public JumpAttack(NormalEnemyCharacter enemyCharacter,float detectRange, float jumpHeight, float preMoveTime,
            float postMoveTime, float distanceOffset, float maxDistance)
            => (this.enemyCharacter, this.detectRange, this.jumpHeight, this.preMoveTime, this.postMoveTime, this.distanceOffset, this.maxDistance) =
                (enemyCharacter, detectRange, jumpHeight, preMoveTime, postMoveTime, distanceOffset, maxDistance);
        
        public override NodeState Evaluate()
        {
            if (!DetectPlayer() && enemyCharacter.curState != NormalEnemyStates.JUMPATTACK)
            {
                state = NodeState.FAILURE;
                return state;
            }
            if (enemyCharacter.curState != NormalEnemyStates.JUMPATTACK)
            {
                // Enter move
                enemyCharacter.curState = NormalEnemyStates.JUMPATTACK;
                enemyCharacter.FacingPlayer();
                // moveTarget = enemyCharacter.player.transform;
                preMoveCountDown = preMoveTime;
                postMoveCountDown = postMoveTime;
                moveIndex = 0;
                enemyCharacter.GeneralIdle();
                enemyCharacter.anim.SetTrigger("JumpAttackStart");
            }
            
            if (moveIndex == 0)
            {
                state = NodeState.RUNNING; // 设为Running不会被打断
                if (preMoveCountDown <= 0)
                {
                    moveIndex = 1;
                    enemyCharacter.disableGroundCheck(.5f);
                    enemyCharacter.anim.SetTrigger("JumpAttackAir");
                    float distanceToPlayer = enemyCharacter.player.transform.position.x - enemyCharacter.transform.position.x;
                    if (distanceToPlayer < 0)
                        distanceToPlayer += distanceOffset;
                    else
                        distanceToPlayer -= distanceOffset;
                    if (distanceToPlayer > maxDistance)
                        distanceToPlayer = maxDistance;
                    if (distanceToPlayer < -maxDistance)
                        distanceToPlayer = -maxDistance;
                    enemyCharacter.rb.AddForce(new Vector2(distanceToPlayer * enemyCharacter.rb.gravityScale, jumpHeight), ForceMode2D.Impulse);
                }
                else
                    preMoveCountDown -= Time.deltaTime;
                enemyCharacter.FacingPlayer();
            }
            else if (moveIndex == 1)
            {
                state = NodeState.SUCCESS; // 设为Running不会被打断， 改为SUCCESS会被打断
                if (enemyCharacter.isGrounded)
                {
                    moveIndex = 2;
                    enemyCharacter.GeneralIdle();
                    enemyCharacter.anim.SetTrigger("JumpAttackPost");
                }
            }
            else if (moveIndex == 2)
            {
                state = NodeState.SUCCESS; // 设为SUCCESS会被打断
                if (postMoveCountDown <= 0)
                {
                    enemyCharacter.curState = NormalEnemyStates.NULL;
                    enemyCharacter.anim.SetTrigger("JumpAttackFinish");
                }
                else
                    postMoveCountDown -= Time.deltaTime;
            }

            enemyCharacter.EdgeProtection();
            return state;
        }
        
        private bool DetectPlayer()
        {
            return (enemyCharacter.CollisionCheck(Vector2.right, detectRange, enemyCharacter.playerLayer))
                   || (enemyCharacter.CollisionCheck(Vector2.left, detectRange, enemyCharacter.playerLayer));
        }
    }
    
}
