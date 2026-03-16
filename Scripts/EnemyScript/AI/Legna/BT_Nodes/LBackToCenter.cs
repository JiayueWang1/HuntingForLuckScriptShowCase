using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

namespace MetroidvaniaTools
{
    public class LBackToCenter : Node
    {
        // Passing in
        private LegnaCharacter character;
        private float distance;
        
        // Local
        private bool centerIsOnLeft;
        private float dir;
        private int moveIndex;
        
        public LBackToCenter(LegnaCharacter character, float distance)
            => (this.character, this.distance) =
                (character, distance);
        
        public override NodeState Evaluate()
        {
            if (character.HandleHit())
            {
                state = NodeState.FAILURE;
                if (character.stunHandled)
                    return state;
                character.GeneralIdle();
                character.Dodge_startTrigger = false;
                character.Dodge_finishTrigger = false;
                character.stunHandled = true;
                character.curState = LegnaStates.NULL;
                return state;
            }
            
            if (character.curState != LegnaStates.POS_ADJUST)
            {
                character.curState = LegnaStates.POS_ADJUST;
                character.FacingPlayer();
                character.GeneralIdle();
                character.Dodge_startTrigger = false;
                character.Dodge_finishTrigger = false;
                // Physics2D.IgnoreCollision(character.col, character.playerCollider, true);
                centerIsOnLeft = character.centerRef.position.x < character.transform.position.x;
                dir = centerIsOnLeft ? -1 : 1;
                if (character.facingLeft == centerIsOnLeft)
                {
                    character.anim.SetTrigger("DodgeFront");
                }
                else
                {
                    character.anim.SetTrigger("DodgeBack");
                }
                moveIndex = 1;
            }
            
            state = NodeState.RUNNING;

            if (moveIndex == 1)
            {
                if (character.Dodge_startTrigger)
                {
                    moveIndex = 2;
                    character.rb.AddForce(new Vector2(dir * distance, 0) * character.rb.mass, ForceMode2D.Impulse);
                }
            }
            else if (moveIndex == 2)
            {
                if (character.Dodge_finishTrigger)
                {
                    character.GeneralIdle();
                    // Physics2D.IgnoreCollision(character.col, character.playerCollider, false);
                    character.curState = LegnaStates.NULL;
                    moveIndex = 0;
                    state = NodeState.SUCCESS;
                }
            }
            
            return state;
        }
    }   
}
