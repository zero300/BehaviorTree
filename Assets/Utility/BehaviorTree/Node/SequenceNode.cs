using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behavior
{
    public class SequenceNode : CompositeNode
    {
        [HideInInspector] public int current;
        protected override void OnStart()
        {
            current = 0;
        }

        protected override void OnStop()
        {
        }

        protected override BTNodeState OnUpdate()
        {
            switch (children[current].Update()) {
                case BTNodeState.Running:
                    return BTNodeState.Running;
                case BTNodeState.Failure:
                    return BTNodeState.Failure;
                case BTNodeState.Success:
                    current++;
                    break;
                default:
                    throw new System.Exception("Type Error");
            }
            if (current < children.Count) return BTNodeState.Running;
            else return BTNodeState.Success;
        }
    }
}