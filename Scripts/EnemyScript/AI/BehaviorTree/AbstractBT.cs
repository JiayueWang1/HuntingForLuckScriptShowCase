using System;
using System.Collections;
using System.Collections.Generic;
using MetroidvaniaTools;
using UnityEngine;

namespace BehaviorTree
{
    public abstract class AbstractBT : MonoBehaviour
    {

        private Node _root = null;

        protected void Start()
        {
            Initialization();
        }

        protected virtual void Initialization()
        {
            _root = SetupTree();
        }

        private void FixedUpdate()
        {
            if (_root != null)
                _root.Evaluate();
        }

        protected abstract Node SetupTree();

    }
}