using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Behavior
{ 
    public enum BTNodeState
    {
        Running,
        Success,
        Failure
    }
    public abstract class BTNode : ScriptableObject
    {
        public BTNodeState state = BTNodeState.Running;
        [TextArea] public string description;
        [HideInInspector] public BlackBoard blackBoard;
        // [HideInInspector] public AiAgent // 教學有 但我沒看到
        [HideInInspector] public Vector2 position;
        [HideInInspector] public bool started = false;
        [HideInInspector] public string guid;

        public BTNodeState Update()
        {
            if (!started)
            {
                OnStart();
                started = true;
            }

            state = OnUpdate();

            if(state == BTNodeState.Success || state == BTNodeState.Failure) {
                OnStop();
                started = false;
            }

            return state;
        }
        protected abstract void OnStart();
        protected abstract BTNodeState OnUpdate();
        protected abstract void OnStop();
        public virtual BTNode Clone()
        {
            return Instantiate(this);
        }
    }
}


