using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
    public class RandomPicker : Node
    {
        private int curNodeIndex;

        public RandomPicker() : base() { 
            curNodeIndex = -1;
        }

        public RandomPicker(List<Node> children) : base(children)
        {
            curNodeIndex = -1;
        }
        
        public override NodeState Evaluate()
        {
            if (curNodeIndex < 0)
                curNodeIndex = GetRandomIndex();
            
            state = children[curNodeIndex].Evaluate();
            if (state != NodeState.RUNNING)
                curNodeIndex = GetRandomIndex();
            return state;
        }

        private int GetRandomIndex()
        {
            return Random.Range(0, children.Count);
        }
    }   
}
