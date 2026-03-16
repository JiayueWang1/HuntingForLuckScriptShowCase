using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

namespace MetroidvaniaTools
{
    public class LBackFire : Node
{
    // Passing in
        private LegnaCharacter character;
        private float distance;
        
        // Local
        private float dir;
        private int moveIndex;
        private bool centerIsOnLeft;
        
        public LBackFire(LegnaCharacter character, float distance)
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
            
            if (character.curState != LegnaStates.FIRE)
            {
                character.curState = LegnaStates.FIRE;
                character.GeneralIdle();
                character.Dodge_startTrigger = false;
                character.Dodge_finishTrigger = false;
                centerIsOnLeft = character.centerRef.position.x < character.transform.position.x;
                dir = centerIsOnLeft ? -1 : 1;
                if (character.facingLeft == centerIsOnLeft)
                {
                    character.Flip();
                }
                character.anim.SetTrigger("BackFire");
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
                    character.curState = LegnaStates.NULL;
                    state = NodeState.SUCCESS;
                }
            }
            
            return state;
        }
}
}

