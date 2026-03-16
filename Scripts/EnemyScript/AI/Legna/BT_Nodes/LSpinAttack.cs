using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

namespace MetroidvaniaTools
{
    public class LSpinAttack : Node
    {
        // Passing in
        private LegnaCharacter character;
        private float slowSpinTime;
        private float slowMaxSpeed;
        private float fastSpinTime;
        private float fastMaxSpeed;
        
        // Local
        private int moveIndex = 0;
        private float slowSpinTimeCountDown;
        private float fastSpinTimeCountDown;
        
        public LSpinAttack(LegnaCharacter character, float slowSpinTime, float slowMaxSpeed, float fastSpinTime, float fastMaxSpeed)
            => (this.character, this.slowSpinTime, this.slowMaxSpeed, this.fastSpinTime, this.fastMaxSpeed) =
                (character, slowSpinTime, slowMaxSpeed, fastSpinTime, fastMaxSpeed);
        
        public override NodeState Evaluate()
        {
            
            if (character.HandleHit())
            {
                state = NodeState.FAILURE;
                if (character.stunHandled)
                    return state;
                character.GeneralIdle();
                character.HitBox.GetComponent<GeneralEnemyTrigger>().alreadyHit = false;
                character.SA_dashTrigger = false;
                character.SA_finishTrigger = false;
                character.HitBox.GetComponent<Animator>().SetBool("SA", false);
                character.curState = LegnaStates.NULL;
                character.stunHandled = true;
                return state;
            }
            
            
            if (character.curState != LegnaStates.SPINATTACK)
            {
                character.curState = LegnaStates.SPINATTACK;
                character.GeneralIdle();
                character.anim.SetTrigger("SpinAttackStart");
                character.FacingPlayer();
                moveIndex = 1;
                character.HitBox.GetComponent<GeneralEnemyTrigger>().alreadyHit = false;
                character.SA_dashTrigger = false;
                character.SA_finishTrigger = false;
            }
            
            state = NodeState.RUNNING;
            if (moveIndex == 1)
            {
                if (character.SA_dashTrigger)
                {
                    character.SA_dashTrigger = false;
                    character.FacingPlayer();
                    slowSpinTimeCountDown = slowSpinTime;
                    // Physics2D.IgnoreCollision(character.col, character.playerCollider, true);
                    character.HitBox.GetComponent<Animator>().SetBool("SA", true);
                    moveIndex = 2;
                }
            }
            else if (moveIndex == 2)
            {
                if (slowSpinTimeCountDown <= 0 || character.HitBox.GetComponent<GeneralEnemyTrigger>().alreadyHit)
                {
                    moveIndex = 3;
                    fastSpinTimeCountDown = fastSpinTime;
                }
                else
                {
                    character.GeneralMovement(slowSpinTime, slowMaxSpeed);
                    slowSpinTimeCountDown -= Time.deltaTime;
                }
            }
            else if (moveIndex == 3)
            {
                if (fastSpinTimeCountDown <= 0 || character.HitBox.GetComponent<GeneralEnemyTrigger>().alreadyHit)
                {
                    moveIndex = 4;
                    character.GeneralIdle();
                    character.HitBox.GetComponent<Animator>().SetBool("SA", false);
                    character.anim.SetTrigger("SpinAttackFinish");
                }
                else
                {
                    character.GeneralMovement(.5f, fastMaxSpeed);
                    fastSpinTimeCountDown -= Time.deltaTime;
                }
            }
            else if (moveIndex == 4)
            {
                if (character.SA_finishTrigger)
                {
                    character.SA_finishTrigger = false;
                    // Physics2D.IgnoreCollision(character.col, character.playerCollider, false);
                    character.curState = LegnaStates.NULL;
                    state = NodeState.SUCCESS;
                }
            }
            
            return state;
        }
    }   
}
