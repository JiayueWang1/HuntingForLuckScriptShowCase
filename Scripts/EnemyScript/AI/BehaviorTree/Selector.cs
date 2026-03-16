using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace BehaviorTree
{
    public class Selector : Node
    {
        private int curNodeIndex;

        public Selector(List<Node> children) : base(children) { curNodeIndex = 0; }
        
        public override NodeState Evaluate()
        {
            if (curNodeIndex >= children.Count)
            {
                Debug.LogError("Unexpected evaluate for selector at index " + curNodeIndex);
                return NodeState.FAILURE;
            }
            
            state = children[curNodeIndex].Evaluate();
            if (state != NodeState.FAILURE)
            {
                if (state == NodeState.SUCCESS)
                    curNodeIndex = 0;
                return state;
            }

            curNodeIndex++;
            while (curNodeIndex < children.Count)
            {
                switch (children[curNodeIndex].Evaluate())
                {
                    case NodeState.FAILURE:
                        break;
                    case NodeState.SUCCESS:
                        state = NodeState.SUCCESS;
                        curNodeIndex = 0;
                        return state;
                    case NodeState.RUNNING:
                        state = NodeState.RUNNING;
                        return state;
                    default:
                        break;
                }
                curNodeIndex++;
            }

            curNodeIndex = 0;
            state = NodeState.FAILURE;
            return state;
        }

    }

}
