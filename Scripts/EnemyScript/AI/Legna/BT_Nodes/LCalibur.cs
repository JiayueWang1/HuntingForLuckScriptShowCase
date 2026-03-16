using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

namespace MetroidvaniaTools
{
    public class LCalibur : Node
    {
        // Pass In
        private LegnaCharacter character;
        private float maxChargeTime;
        private float minDistance;
        private float velocityOffset;
        private float maxDistanceToApply;
        
        // Local
        private int moveIndex = 0;
        private float chargeCountDown;
        private HorizontalMovement playerHorizontalMovement;
        private bool effectTriggered;
        public LCalibur(LegnaCharacter character, float maxChargeTime, float minDistance,
        float velocityOffset, float maxDistanceToApply)
            => (this.character, this.maxChargeTime, this.minDistance, this.velocityOffset, this.maxDistanceToApply) =
                (character, maxChargeTime, minDistance, velocityOffset, maxDistanceToApply);

        public override NodeState Evaluate()
        {
            if (character.HandleHit())
            {
                state = NodeState.FAILURE;
                if (character.stunHandled)
                    return state;
                character.GeneralIdle();
                character.NA_finishTrigger = false;
                chargeCountDown = maxChargeTime;
                effectTriggered = false;
                character.playerHorizontalMovement.velocityOffset = 0;
                character.caliburCharge.SetActive(false);
                character.suck.SetActive(false);
                character.curState = LegnaStates.NULL;
                character.stunHandled = true;
                return state;
            }
            
            if (character.curState != LegnaStates.EXCALIBUR)
            {
                // Enter move
                character.curState = LegnaStates.EXCALIBUR;
                character.FacingPlayer();
                character.GeneralIdle();
                character.NA_finishTrigger = false;
                chargeCountDown = maxChargeTime;
                effectTriggered = false;
                character.caliburCharge.SetActive(false);
                character.suck.SetActive(false);
                character.anim.SetTrigger("CaliburStart");
                moveIndex = 1;
            }

            state = NodeState.RUNNING;

            if (moveIndex == 1)
            {
                if (character.NA_finishTrigger)
                {
                    character.NA_finishTrigger = false;
                    character.FacingPlayer();
                    moveIndex = 2;
                }
            }
            else if (moveIndex == 2)
            {
                if (chargeCountDown <= 0 || character.GetPlayerDistance() < minDistance)
                {
                    character.anim.SetTrigger("CaliburEnd");
                    character.playerHorizontalMovement.velocityOffset = 0;
                    character.caliburCharge.SetActive(false);
                    character.suck.SetActive(false);
                    moveIndex = 3;
                }
                else
                {
                    bool playerIsOnLeft = character.player.transform.position.x <= character.transform.position.x;
                    if (character.facingLeft == playerIsOnLeft && character.GetPlayerDistance() < maxDistanceToApply)
                    {
                        float dir = playerIsOnLeft ? 1 : -1;
                        character.playerHorizontalMovement.velocityOffset = dir * velocityOffset;
                    }
                    else
                    {
                        character.playerHorizontalMovement.velocityOffset = 0;
                    }

                    if (!effectTriggered)
                    {
                        character.caliburCharge.SetActive(true);
                        character.suck.SetActive(true);
                    }
                    
                    chargeCountDown -= Time.deltaTime;
                }
            }
            else if (moveIndex == 3)
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
