using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

namespace MetroidvaniaTools
{
    public class LSlashAttack2C : Node
    {
        // Pass In
        private LegnaCharacter character;
        private float s2JumpDistance;
        
        private int moveIndex = 0;
        public LSlashAttack2C(LegnaCharacter character, float s2JumpDistance)
            => (this.character, this.s2JumpDistance) =
                (character, s2JumpDistance);

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
                    character.anim.SetTrigger("SlashAttackEnd");
                    character.curState = LegnaStates.NULL;
                    state = NodeState.SUCCESS;
                }
            }

            return state;
        }
    }   
}
