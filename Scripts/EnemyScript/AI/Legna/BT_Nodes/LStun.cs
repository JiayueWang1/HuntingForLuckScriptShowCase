using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

namespace MetroidvaniaTools
{
    public class LStun : Node
    {
        // Passing in
        private LegnaCharacter character;
        private float stunTime;

        // Local
        private int moveIndex;
        private float stunCountDown;
        public LStun(LegnaCharacter character, float stunTime)
            => (this.character, this.stunTime) =
                (character, stunTime);
        
        public override NodeState Evaluate()
        {
            if (character.curState != LegnaStates.STUN)
            {
                character.curState = LegnaStates.STUN;
                character.FacingPlayer();
                character.GeneralIdle();
                stunCountDown = stunTime;
                character.HitBox.GetComponent<Animator>().SetBool("SA", false);
                character.HitBox.GetComponent<Animator>().SetTrigger("CANCLE");
                character.stunHandled = false;
                character.ST_endTrigger = false;
                character.anim.SetTrigger("StunStart");
                moveIndex = 1;
            }
            
            character.HandleHit();
            character.isStagger = false;
            state = NodeState.RUNNING;
            if (moveIndex == 1)
            {
                if (stunCountDown <= 0)
                {
                    character.anim.SetTrigger("StunEnd");
                    character.FacingPlayer();
                    moveIndex = 2;
                }
                else
                {
                    stunCountDown -= Time.deltaTime;
                }
            }
            if (moveIndex == 2)
            {
                if (character.ST_endTrigger)
                {
                    character.ST_endTrigger = false;
                    if (character.health.phaseIndex == 1)
                    {
                        character.toughness = character.p1_MaxToughness;
                    }
                    else
                    {
                        character.toughness = character.p2_MaxToughness;
                    }
                    character.curState = LegnaStates.NULL;
                    state = NodeState.SUCCESS;
                }
            }
            return state;
        }
    }    
}

