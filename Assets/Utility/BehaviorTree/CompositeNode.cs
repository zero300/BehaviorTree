using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.VisualScripting.Metadata;

namespace Behavior
{
    public abstract class CompositeNode : BTNode
    {
        public List<BTNode> children = new List<BTNode>();

        public override BTNode Clone()
        {
            CompositeNode node = Instantiate(this);
            node.children = children.ConvertAll(x => x.Clone());
            return node;
        }
    }

}