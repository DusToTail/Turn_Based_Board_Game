using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitNode : ActionNode
{
    public float duration = 1;
    private float _start;
    protected override void OnStart()
    {
        _start = Time.time;
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        if(Time.time - _start > duration)
            return State.Success;
        return State.Running;
    }
}
