using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
    public class Successor : Node
    {
        public Successor(Node children) : base()
        {
            _Attach(children);
        }

        public override NodeState Evaluate()
        {
            children[0].Evaluate();
            return NodeState.SUCCESS;
        }
        
    }
}
