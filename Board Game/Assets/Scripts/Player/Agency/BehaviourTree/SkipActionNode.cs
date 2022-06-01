using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkipActionNode : ActionNode
{
    public CharacterBlock self;

    protected override void OnStart()
    {
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        self.SkipAction();
        return State.Success;
    }

}
