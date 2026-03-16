using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

namespace MetroidvaniaTools
{
    public class LTwinkleAttack : Node
    {
        // Pass In
        private LegnaCharacter character;
        private float intervalTime;
        
        // Local
        private int moveIndex = 0;
        private float fallDownCountDown;
        private bool endEffectTriggered;
        public LTwinkleAttack(LegnaCharacter character, float intervalTime)
            => (this.character, this.intervalTime) =
                (character, intervalTime);

        public override NodeState Evaluate()
        {
            if (character.HandleHit())
            {
                state = NodeState.FAILURE;
                if (character.stunHandled)
                    return state;
                character.GeneralIdle();
                character.TA_prepareEnd = false;
                character.TA_endTrigger = false;
                endEffectTriggered = false;
                fallDownCountDown = intervalTime;
                character.curState = LegnaStates.NULL;
                character.stunHandled = true;
                return state;
            }
            
            if (character.curState != LegnaStates.TWINKLEATTACK)
            {
                // Enter move
                character.curState = LegnaStates.TWINKLEATTACK;
                character.FacingPlayer();
                character.GeneralIdle();
                character.TA_prepareEnd = false;
                character.TA_endTrigger = false;
                endEffectTriggered = false;
                fallDownCountDown = intervalTime;
                character.anim.SetTrigger("TwinkleAttack");
                moveIndex = 1;
            }

            state = NodeState.RUNNING;
            if (moveIndex == 1)
            {
                if (character.TA_prepareEnd)
                {
                    character.TA_prepareEnd = false;
                    character.col.enabled = false;
                    character.rb.simulated = false;
                    character.TriggerTwinkleStart();
                    moveIndex = 2;
                }
            }
            else if (moveIndex == 2)
            {
                character.transform.position = character.player.transform.position + Vector3.up;

                if (!endEffectTriggered)
                {
                    endEffectTriggered = true;
                    character.TriggerTwinkleEnd();
                }
                
                if (fallDownCountDown <= 0)
                {
                    character.anim.SetTrigger("TwinkleAttackDown");
                    character.col.enabled = true;
                    character.rb.simulated = true;
                    moveIndex = 3;
                }
                else
                    fallDownCountDown -= Time.deltaTime;
            }
            else if (moveIndex == 3)
            {
                if (character.TA_endTrigger)
                {
                    character.curState = LegnaStates.NULL;
                    state = NodeState.SUCCESS;
                }
            }

            return state;
        }
    }   
}
