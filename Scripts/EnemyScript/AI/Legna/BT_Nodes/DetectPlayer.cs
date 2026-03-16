using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

namespace MetroidvaniaTools
{
    public class DetectPlayer : Node
    {
        private LegnaCharacter character;
        private int distanceClass;

        private bool running;

        public DetectPlayer(LegnaCharacter character, int distanceClass, Node child)
        {
            this.character = character;
            this.distanceClass = distanceClass;
            _Attach(child);
        }
        
        
        public override NodeState Evaluate()
        {

            // if (character.HandleHit())
            // {
            //     running = false;
            //     state = NodeState.FAILURE;
            //     return state;
            // }
            
            if (!running && distanceClass != character.playerDistanceClass)
            {
                state = NodeState.FAILURE;
                return state;
            }
            
            state = children[0].Evaluate();
            if (state == NodeState.RUNNING)
                running = true;
            else
            {
                // state = NodeState.SUCCESS;
                running = false;
            }
            
            return state;
            
        }
        
    }   
}
