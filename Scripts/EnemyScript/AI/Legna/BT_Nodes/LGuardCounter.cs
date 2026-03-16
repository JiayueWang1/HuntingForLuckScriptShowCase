using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

namespace MetroidvaniaTools
{
    public class LGuardCounter : Node
    {
        // Pass In
        private LegnaCharacter character;
        private float maxGuardTime;
        private float maxGuardDistance;
        private float closeDistance;
        private float dodgeForce;
        
        
        private int moveIndex = 0;
        private float counterStartCountDown;
        private float guardEndCountDown;
        private bool centerIsOnLeft;
        private float dir;
        
        public LGuardCounter(LegnaCharacter character, float maxGuardTime, float maxGuardDistance, float closeDistance, float dodgeForce)
            => (this.character, this.maxGuardTime, this.maxGuardDistance, this.closeDistance, this.dodgeForce) =
                (character, maxGuardTime, maxGuardDistance, closeDistance, dodgeForce);

        public override NodeState Evaluate()
        {
            if (character.HandleHit())
            {
                state = NodeState.FAILURE;
                if (character.stunHandled)
                    return state;
                character.GeneralIdle();
                character.health.isGuarding = false;
                character.Guard_startTrigger = false;
                character.Guard_endTrigger = false;
                character.Counter_startTrigger = false;
                character.Counter_finishTrigger = false;
                character.guardCancelTriggered = false;
                guardEndCountDown = maxGuardTime;
                character.shield.SetActive(false);
                character.curState = LegnaStates.NULL;
                character.stunHandled = true;
                return state;
            }
            
            
            if (character.curState != LegnaStates.GUARD_COUNTER)
            {
                if (((float)character.health.healthPoints / character.health.maxHealthPoints) > .5f)
                {
                    state = NodeState.FAILURE;
                    return state;
                }
                
                float randFloat = Random.Range(0, (float)character.health.maxHealthPoints);
                if (randFloat < character.health.healthPoints)
                {
                    state = NodeState.FAILURE;
                    return state;
                }
                
                
                // Enter move
                character.curState = LegnaStates.GUARD_COUNTER;
                character.FacingPlayer();
                character.GeneralIdle();
                character.health.isGuarding = false;
                character.Guard_startTrigger = false;
                character.Guard_endTrigger = false;
                character.Counter_startTrigger = false;
                character.Counter_finishTrigger = false;
                character.guardCancelTriggered = false;
                character.shield.SetActive(false);
                guardEndCountDown = maxGuardTime;
                character.anim.SetTrigger("GuardCounterStart");
                moveIndex = 1;
            }

            state = NodeState.RUNNING;
            if (moveIndex == 1)
            {
                // 前摇
                character.FacingPlayer();
                character.shieldCheck();
                guardEndCountDown -= Time.deltaTime;
                if (character.Guard_startTrigger)
                {
                    character.Guard_startTrigger = false;
                    character.health.isGuarding = true;
                    moveIndex = 2;
                }
            }
            else if (moveIndex == 2)
            {
                // 防御中
                character.shieldCheck();
                if (character.Guard_endTrigger || guardEndCountDown <= 0)
                {
                    character.Guard_endTrigger = false;
                    counterStartCountDown = .1f;
                    moveIndex = 3;
                    return state;
                }
                
                bool playerIsOnLeft = character.player.transform.position.x < character.transform.position.x;
                if (character.facingLeft != playerIsOnLeft)
                {
                    character.health.isGuarding = false;
                    character.shield.SetActive(false);
                    character.guardCancelTriggered = false;
                    character.anim.SetTrigger("GuardCounterStart");
                    moveIndex = 1;
                    return state;
                }
                
                float curDist = character.GetPlayerDistance();
                if (curDist > maxGuardDistance)
                {
                    character.health.isGuarding = false;
                    character.shield.SetActive(false);
                    character.guardCancelTriggered = false;
                    character.anim.SetTrigger("GuardEnd");
                    character.curState = LegnaStates.NULL;
                    state = NodeState.SUCCESS;
                    return state;
                }

                if (curDist < closeDistance && character.playerDistanceClass == 0)
                {
                    character.FacingPlayer();
                    character.Dodge_startTrigger = false;
                    character.Dodge_finishTrigger = false;
                    character.health.isGuarding = false;
                    character.shield.SetActive(false);
                    character.guardCancelTriggered = false;
                    
                    dir = character.facingLeft ? -1 : 1;
                    character.anim.SetTrigger("DodgeFront");
                    moveIndex = 5;
                    return state;
                }

                guardEndCountDown -= Time.deltaTime;
            }
            else if (moveIndex == 3)
            {
                // 反击延迟
                if (counterStartCountDown <= 0)
                {
                    character.FacingPlayer();
                    character.health.isGuarding = false;
                    character.shield.SetActive(false);
                    character.guardCancelTriggered = false;
                    character.anim.SetTrigger("CounterStart");
                    moveIndex = 4;
                }
                else
                    counterStartCountDown -= Time.deltaTime;
            }
            else if (moveIndex == 4)
            {
                // 反击中
                character.Counter_startTrigger = false;
                if (character.Counter_finishTrigger)
                {
                    character.Counter_finishTrigger = false;
                    character.curState = LegnaStates.NULL;
                    state = NodeState.SUCCESS;
                    return state;
                }
            }
            else if (moveIndex == 5)
            {
                // 移动前摇
                if (character.Dodge_startTrigger)
                {
                    moveIndex = 6;
                    character.rb.AddForce(new Vector2(dir * dodgeForce, 0) * character.rb.mass, ForceMode2D.Impulse);
                }
            }
            else if (moveIndex == 6)
            {
                // 移动+反击
                if (character.Counter_startTrigger)
                {
                    character.FacingPlayer();
                    character.GeneralIdle();
                    
                }
                if (character.Counter_finishTrigger)
                {
                    character.Counter_finishTrigger = false;
                    character.curState = LegnaStates.NULL;
                    state = NodeState.SUCCESS;
                }
            }

            return state;
        }
    }   
}
