using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Behavior;

public class WaitNode : ActionNode
{
    // µ¥«Ý
    public float duration = 1.0f;
    float passTime; 
    protected override void OnStart()
    {
        passTime = duration;
    }

    protected override void OnStop()
    {
        passTime = 0.0f;
    }

    protected override BTNodeState OnUpdate()
    {
        passTime -= Time.deltaTime;

        if (passTime <= 0.0f) return BTNodeState.Success;
        else return BTNodeState.Running;
    }
}
