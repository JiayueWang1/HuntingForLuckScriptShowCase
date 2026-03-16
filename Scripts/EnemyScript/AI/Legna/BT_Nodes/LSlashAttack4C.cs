using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

namespace MetroidvaniaTools
{
    public class LSlashAttack4C : Node
    {
        // Pass In
        private LegnaCharacter character;
        private float s2JumpDistance;
        private float s3JumpDistance;
        
        private int moveIndex = 0;
        public LSlashAttack4C(LegnaCharacter character, float s2JumpDistance, float s3JumpDistance)
            => (this.character, this.s2JumpDistance, this.s3JumpDistance) =
                (character, s2JumpDistance, s3JumpDistance);

        public override NodeState Evaluate()
        {
            if (character.HandleHit())
            {
                state = NodeState.FAILURE;
                if (character.stunHandled)
                    return state;
                character.GeneralIdle();
                character.NA_finishTrigger = false;
                character.curState = LegnaStates.NULL;
                character.stunHandled = true;
                return state;
            }
            
            if (character.curState != LegnaStates.CROSSATTACK)
            {
                // Enter move
                character.curState = LegnaStates.CROSSATTACK;
                character.FacingPlayer();
                character.GeneralIdle();
                character.NA_finishTrigger = false;
                character.anim.SetTrigger("SlashAttackStart");
                moveIndex = 1;
            }

            state = NodeState.RUNNING;

            if (moveIndex == 1)
            {
                if (character.NA_finishTrigger)
                {
                    character.NA_finishTrigger = false;
                    character.FacingPlayer();
                    float dir = character.facingLeft ? -1 : 1;
                    character.rb.AddForce(new Vector2(dir * s2JumpDistance, 0) * character.rb.mass, ForceMode2D.Impulse);
                    moveIndex = 2;
                }
            }
            else if (moveIndex == 2)
            {
                if (character.NA_finishTrigger)
                {
                    character.NA_finishTrigger = false;
                    character.GeneralIdle();
                    character.FacingPlayer();
                    float dir = character.facingLeft ? 1 : -1;
                    character.rb.AddForce(new Vector2(dir * s3JumpDistance, 0) * character.rb.mass, ForceMode2D.Impulse);
                    character.anim.SetTrigger("SlashAttack2");
                    moveIndex = 3;
                }
            }
            else if (moveIndex == 3)
            {
                if (character.NA_finishTrigger)
                {
                    character.NA_finishTrigger = false;
                    character.GeneralIdle();
                    character.FacingPlayer();
                    moveIndex = 4;
                }
            }
            else if (moveIndex == 4)
            {
                if (character.NA_finishTrigger)
                {
                    character.NA_finishTrigger = false;
                    character.curState = LegnaStates.NULL;
                    state = NodeState.SUCCESS;
                }
            }

            return state;
        }
    }   
}
