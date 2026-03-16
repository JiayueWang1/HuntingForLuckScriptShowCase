using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

namespace MetroidvaniaTools
{
    public class PhaseChecker : Node
    {
        private LegnaCharacter character;
        private int phaseNum;

        public PhaseChecker(LegnaCharacter character, int phaseNum, Node child)
        {
            this.character = character;
            this.phaseNum = phaseNum;
            _Attach(child);
        }
        
        
        public override NodeState Evaluate()
        {
            if (character.health.phaseIndex == phaseNum)
            {
                children[0].Evaluate();
                state = NodeState.RUNNING;
            }
            else
            {
                state = NodeState.SUCCESS;
            }
            
            return state;
            
        }
    }   
}
