using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

namespace MetroidvaniaTools
{
    public class LDeath : Node
    {
        // Passing in
        private LegnaCharacter character;
        
        public LDeath(LegnaCharacter character)
            => (this.character) =
                (character);
        
        public override NodeState Evaluate()
        {
            if (character.curState != LegnaStates.DEATH)
            {
                character.curState = LegnaStates.DEATH;
                character.FacingPlayer();
                character.GeneralIdle();
                
                character.stunHandled = false;
                character.shield.SetActive(false);
                character.caliburCharge.SetActive(false);
                character.suck.SetActive(false);
                character.HitBox.GetComponent<Animator>().SetBool("SA", false);
                character.HitBox.GetComponent<Animator>().SetTrigger("CANCLE");
                character.anim.SetBool("Running", false);
                
                character.rb.simulated = false;
                character.col.enabled = false;
                character.anim.SetTrigger("Death");
            }
            state = NodeState.RUNNING;
            return state;
        }
    }
   
}