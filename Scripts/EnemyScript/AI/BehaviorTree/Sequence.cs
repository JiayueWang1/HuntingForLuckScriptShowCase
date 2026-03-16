using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace BehaviorTree
{
    public class Sequence : Node
    {
        private int curNodeIndex;
        public Sequence(List<Node> children) : base(children) { curNodeIndex = 0;  }

        public override NodeState Evaluate()
        {
            if (curNodeIndex >= children.Count)
            {
                Debug.LogError("Unexpected evaluate for sequence at index " + curNodeIndex);
                return NodeState.FAILURE;
            }
            
            state = children[curNodeIndex].Evaluate();
            if (state != NodeState.SUCCESS)
            {
                if (state == NodeState.FAILURE)
                    curNodeIndex = 0;
                return state;
            }

            curNodeIndex++;
            while (curNodeIndex < children.Count)
            {
                switch (children[curNodeIndex].Evaluate())
                {
                    case NodeState.FAILURE:
                        state = NodeState.FAILURE;
                        curNodeIndex = 0;
                        return state;
                    case NodeState.SUCCESS:
                        break;
                    case NodeState.RUNNING:
                        state = NodeState.RUNNING;
                        return state;
                    default:
                        state = NodeState.SUCCESS;
                        return state;
                }
                curNodeIndex++;
            }
            
            curNodeIndex = 0;
            state = NodeState.SUCCESS;
            return state;
        }

    }

}