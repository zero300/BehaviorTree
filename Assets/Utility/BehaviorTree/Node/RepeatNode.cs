using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Behavior
{
    public class RepeatNode : DecoratorNode
    {

        protected override void OnStart()
        {
            
        }

        protected override void OnStop()
        {
            
        }

        protected override BTNodeState OnUpdate()
        {
            child.Update();
            return BTNodeState.Running;
        }
    }
}
