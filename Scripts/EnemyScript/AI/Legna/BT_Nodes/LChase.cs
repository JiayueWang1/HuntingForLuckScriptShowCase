using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

namespace MetroidvaniaTools
{
    public class LChase : Node
    {
        // Passing in
        private LegnaCharacter character;
        private float timeTillMaxSpeed;
        private float maxSpeed;
        private float successDistance;
        
        public LChase(LegnaCharacter character, float timeTillMaxSpeed, float maxSpeed, float successDistance)
            => (this.character, this.timeTillMaxSpeed, this.maxSpeed, this.successDistance) =
                (character, timeTillMaxSpeed, maxSpeed, successDistance);
        
        public override NodeState Evaluate()
        {
            
            if (character.HandleHit())
            {
                state = NodeState.FAILURE;
                if (character.stunHandled)
                    return state;
                character.GeneralIdle();
                character.anim.SetBool("Running", false);
                character.curState = LegnaStates.NULL;
                character.stunHandled = true;
                return state;
            }
            
            if (character.GetPlayerDistance() < successDistance)
            {
                character.anim.SetBool("Running", false);
                state = NodeState.SUCCESS;
                return state;
            }
            
            if (character.curState != LegnaStates.CHASE)
            {
                character.curState = LegnaStates.CHASE;
                character.anim.SetBool("Running", true);
            }
            character.FacingPlayer();
            character.GeneralMovement(timeTillMaxSpeed, maxSpeed);
            
            state = NodeState.RUNNING;
            return state;
        }
    }   
}
