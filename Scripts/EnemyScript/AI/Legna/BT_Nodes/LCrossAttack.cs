using UnityEngine;
using BehaviorTree;

namespace MetroidvaniaTools
{
    public class LCrossAttack : Node
    {
        // Pass In
        private LegnaCharacter character;
        private float n2Distance;
        
        
        private int moveIndex = 0;
        public LCrossAttack(LegnaCharacter character,float n2Distance)
            => (this.character, this.n2Distance) =
                (character, n2Distance);

        public override NodeState Evaluate()
        {
            if (character.HandleHit())
            {
                state = NodeState.FAILURE;
                if (character.stunHandled)
                    return state;
                character.GeneralIdle();
                character.NA_finishTrigger = false;
                moveIndex = 0;
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
                character.anim.SetTrigger("CrossAttackStart");
                moveIndex = 1;
            }

            state = NodeState.RUNNING;
            if (character.NA_finishTrigger)
            {
                character.NA_finishTrigger = false;

                int rand = Random.Range(0, 4);
                if (moveIndex == 3 || character.GetPlayerDistance() > n2Distance || rand == 0)
                {
                    character.curState = LegnaStates.NULL;
                    character.anim.SetTrigger("CrossAttackFinish");
                    moveIndex = 0;
                    character.curState = LegnaStates.NULL;
                    state = NodeState.SUCCESS;
                }
                else
                {
                    moveIndex++;
                    character.FacingPlayer();
                    character.GeneralIdle();
                    if (moveIndex == 2)
                        character.anim.SetTrigger("CrossAttack2");
                    if (moveIndex == 3)
                        character.anim.SetTrigger("CrossAttack3");
                }
            }

            return state;
        }
    }
}
