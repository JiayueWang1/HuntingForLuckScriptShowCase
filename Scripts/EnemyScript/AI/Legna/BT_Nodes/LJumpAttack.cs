using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

namespace MetroidvaniaTools
{
    public class LJumpAttack : Node
    {
        private LegnaCharacter character;
        private float jumpHeight;
        private float distanceOffset;
        private float maxDistance;
        
        // Local
        private float preMoveCountDown;
        private float postMoveCountDown;
        private int moveIndex; // 0 for premove, 1 for air, 2 for postmove

        public LJumpAttack(LegnaCharacter character, float jumpHeight, float distanceOffset, float maxDistance)
            => (this.character, this.jumpHeight, this.distanceOffset, this.maxDistance) =
                (character, jumpHeight, distanceOffset, maxDistance);
        
        public override NodeState Evaluate()
        {
            if (character.HandleHit())
            {
                state = NodeState.FAILURE;
                if (character.stunHandled)
                    return state;
                character.GeneralIdle();
                character.JA_airTrigger = false;
                character.JA_finishTrigger = false;
                moveIndex = 0;
                character.curState = LegnaStates.NULL;
                character.stunHandled = true;
                return state;
            }
            
            if (character.curState != LegnaStates.JUMPATTACK)
            {
                // Enter move
                character.curState = LegnaStates.JUMPATTACK;
                character.FacingPlayer();
                character.JA_airTrigger = false;
                character.JA_finishTrigger = false;
                moveIndex = 0;
                character.GeneralIdle();
                // Physics2D.IgnoreCollision(character.col, character.playerCollider, true);
                character.anim.SetTrigger("JumpAttackStart");
            }
            
            state = NodeState.RUNNING;
            
            if (moveIndex == 0)
            {
                if (character.JA_airTrigger)
                {
                    character.JA_airTrigger = false;
                    moveIndex = 1;
                    character.disableGroundCheck(.5f);
                    float distanceToPlayer = character.player.transform.position.x - character.transform.position.x;
                    if (distanceToPlayer < 0)
                        distanceToPlayer += distanceOffset;
                    else
                        distanceToPlayer -= distanceOffset;
                    if (distanceToPlayer > maxDistance)
                        distanceToPlayer = maxDistance;
                    if (distanceToPlayer < -maxDistance)
                        distanceToPlayer = -maxDistance;
                    character.rb.AddForce(new Vector2(distanceToPlayer, jumpHeight) * character.rb.mass, ForceMode2D.Impulse);
                }
                character.FacingPlayer();
            }
            else if (moveIndex == 1)
            {
                if (character.isGrounded)
                {
                    moveIndex = 2;
                    character.GeneralIdle();
                    character.anim.SetTrigger("JumpAttackPerform");
                }
            }
            else if (moveIndex == 2)
            {
                if (character.JA_finishTrigger)
                {
                    character.JA_finishTrigger = false;
                    // Physics2D.IgnoreCollision(character.col, character.playerCollider, false);
                    character.curState = LegnaStates.NULL;
                    state = NodeState.SUCCESS;
                }
            }

            character.EdgeProtection();
            return state;
        }
    }   
}
