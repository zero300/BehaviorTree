using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behavior
{
    public class DebugNode : ActionNode
    {
        public string debugMessage;
        protected override void OnStart()
        {
            Debug.Log($"OnStart : {debugMessage}");
        }

        protected override void OnStop()
        {
            Debug.Log($"OnStop : {debugMessage}");
        }

        protected override BTNodeState OnUpdate()
        {
            Debug.Log($"OnUpdate : {debugMessage}");
            return BTNodeState.Success;
        }
    }
}
