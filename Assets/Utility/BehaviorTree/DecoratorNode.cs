using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behavior
{
    public abstract class DecoratorNode : BTNode
    {
        public BTNode child;

        public override BTNode Clone()
        {
            DecoratorNode node = Instantiate(this);
            node.child = child.Clone();
            return node;
        }
    }

}